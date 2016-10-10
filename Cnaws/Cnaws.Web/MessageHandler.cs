using Cnaws.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace Cnaws.Web
{
    public enum SessionTriggerMode
    {
        /// <summary>
        /// 事件驱动
        /// </summary>
        Event = 0,
        /// <summary>
        /// 轮询
        /// </summary>
        Polling = 1
    }

    public interface IMessage
    {

    }
    public interface IMessageSessionManager
    {
        bool DoSession(MessageRequest request);
    }

    public class MessageHandler : IHttpAsyncHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException();
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            MessageAsyncResult result = new MessageAsyncResult(context, cb, extraData);
            result.HandleRequest();
            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            MessageAsyncResult msg = result as MessageAsyncResult;
            msg.Context.Response.Write(msg.ToJson());
        }
    }
    internal sealed class MessageAsyncResult : IAsyncResult
    {
        private HttpContext _context;
        private AsyncCallback _callback;
        private object _data;
        private MessageRequest _request;

        internal MessageAsyncResult(HttpContext context, AsyncCallback cb, object extraData)
        {
            _context = context;
            _callback = cb;
            _data = extraData;
            _request = new MessageRequest(this);
        }

        public HttpContext Context
        {
            get { return _context; }
        }

        public object AsyncState
        {
            get { return _data; }
        }
        public WaitHandle AsyncWaitHandle
        {
            get { throw new InvalidOperationException(); }
        }
        public bool CompletedSynchronously
        {
            get { return false; }
        }
        public bool IsCompleted
        {
            get { return _request.IsCompleted; }
        }

        internal void HandleRequest()
        {
            MessageThreadPool.QueueRequest(_request);
        }
        internal void FinishRequest()
        {
            _callback?.Invoke(this);
        }

        internal string ToJson()
        {
            return _request.ToJson();
        }
    }
    public sealed class MessageRequest
    {
        private MessageAsyncResult _result;
        private List<IMessage> _messages;
        private DateTime _begin;
        private bool _completed;
        private bool _timeout;
        private ReaderWriterLockSlim _locker;
        private long _id;
        private int _type;
        private bool _post;
        private string _content;

        internal MessageRequest(MessageAsyncResult result)
        {
            _begin = DateTime.Now;
            _completed = false;
            _timeout = false;
            _result = result;
            _locker = new ReaderWriterLockSlim();
            _messages = new List<IMessage>();
            long.TryParse(result.Context.Request["id"], out _id);
            int.TryParse(result.Context.Request["type"], out _type);
            _post = "POST".Equals(result.Context.Request.HttpMethod);
            if (_post)
                _content = result.Context.Request.Form["content"];
            else
                _content = null;
        }

        internal DateTime BeginTime
        {
            get { return _begin; }
        }
        internal bool IsCompleted
        {
            get { return _completed; }
            set { _completed = value; }
        }
        internal bool IsTimeOut
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public long Id
        {
            get { return _id; }
        }
        public int Type
        {
            get { return _type; }
        }
        public bool IsPost
        {
            get { return _post; }
        }
        public string Content
        {
            get { return _content; }
        }

        /// <summary>
        /// 添加新消息
        /// </summary>
        public void SetMessage(IEnumerable<IMessage> messages)
        {
            _locker.EnterWriteLock();
            try
            {
                _messages.AddRange(messages);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        internal void FinishRequest()
        {
            _result.FinishRequest();
        }

        internal string ToJson()
        {
            _result.Context.Response.ContentType = "application/x-javascript";

            if (_timeout)
                return "{\"code\":-408}";

            _locker.EnterReadLock();
            try
            {
                return string.Concat("{\"code\":-200,\"data\":", JsonValue.Serialize(_messages), '}');
            }
            catch (Exception)
            {
                return "{\"code\":-500}";
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }
    }
    internal sealed class MessageThread
    {
        //兼容浏览器，超时应为20s
        private const int RequestTimeOut = 20;

        // 事件触发模式
        private SessionTriggerMode _mode;
        private IMessageSessionManager _manager;
        private LinkedList<MessageRequest> _requestList;
        private Thread _thread;

        //并发锁
        private ReaderWriterLockSlim RequestLock = new ReaderWriterLockSlim();
        //private ReaderWriterLockSlim MessageLock = new ReaderWriterLockSlim();

        //会话，事件驱动触发
        //private AutoResetEvent SessionWaitHandle = new AutoResetEvent(false);
        //线程无请求是，等待信号
        private AutoResetEvent ThreadWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// 线程初始化
        /// </summary>
        public MessageThread(IMessageSessionManager sessionManager, SessionTriggerMode mode = SessionTriggerMode.Event)
        {
            _requestList = new LinkedList<MessageRequest>();
            //使用事件驱动
            _mode = mode;
            _manager = sessionManager;
            _thread = new Thread(new ThreadStart(MessageThreadStart));
            _thread.IsBackground = false;
            _thread.Start();
        }

        //public int RequestCount
        //{
        //    get { return _requestList.Count * MessageThreadPool.ThreadCount; }
        //}

        private void MessageThreadStart()
        {
            while (true)
            {
                //转成数组再处理，避免长时间对CometRequestList对象 lock
                MessageRequest[] processRequest;
                RequestLock.EnterReadLock();
                try
                {
                    processRequest = new MessageRequest[_requestList.Count];
                    _requestList.CopyTo(processRequest, 0);
                }
                finally
                {
                    RequestLock.ExitReadLock();
                }
                while (processRequest.Length < 1)
                {
                    ThreadWaitHandle.WaitOne();
                    RequestLock.EnterReadLock();
                    try
                    {
                        processRequest = new MessageRequest[_requestList.Count];
                        _requestList.CopyTo(processRequest, 0);
                    }
                    finally
                    {
                        RequestLock.ExitReadLock();
                    }
                }

                //处理请求
                if (_mode == SessionTriggerMode.Event)
                    HandleEventMode(processRequest);
                else
                    HandlePollingMode(processRequest);
            }
        }

        /// <summary>
        /// 以新消息触发 队列请求处理
        /// </summary>
        private void HandleEventMode(MessageRequest[] requests)
        {
            ////1s超时进入轮询
            //SessionWaitHandle.WaitOne(1000);
            //IMessage[] chatMessages;
            //MessageLock.EnterReadLock();
            //try
            //{
            //    //chatMessages = _messages.ToArray();
            //    //内存中值保留前20条记录，避免查询耗时
            //    //_messages = _messages.Take(20).ToList();
            //}
            //finally
            //{
            //    MessageLock.ExitReadLock();
            //}
            //foreach (IMessageRequest request in requests)
            //    SessionManager.DoChatSession(request, chatMessages, FinishCometRequest);

            foreach (MessageRequest request in requests)
            {
                request.IsCompleted = MessageThreadPool.SessionManager.DoSession(request);
                FinishRequest(request);
            }
        }

        /// <summary>
        /// 单轮询模式处理，定时检查消息队列
        /// </summary>
        private void HandlePollingMode(MessageRequest[] requests)
        {
            //IMessage[] chatMessages;
            //lock (MessageSyncRoot)
            //{
            //    chatMessages = _messages.ToArray();
            //    //内存中值保留前20条记录，避免查询耗时
            //    _messages = _messages.Take(20).ToList();
            //}
            //foreach (IMessageRequest request in requests)
            //    SessionManager.DoChatSession(request, chatMessages, FinishCometRequest);
            ////定时扫描
            //Thread.Sleep(200);
            throw new InvalidOperationException();
        }

        ///// <summary>
        ///// 立即处理请求(返回时候得到处理)
        ///// </summary>
        //private void HandleCurrentRequest(MessageRequest request)
        //{
        //    lock (MessageSyncRoot)
        //    {
        //        //处理一个请求，不对MessageList copy了
        //        SessionManager.DoChatSession(request, _messages, null);
        //        if (request.IsCompeled)
        //            request.FinishRequest();
        //    }
        //}

        ///// <summary>
        ///// 添加新消息
        ///// </summary>
        //public void HandleMessage(IMessage message)
        //{
        //    MessageLock.EnterWriteLock();
        //    try
        //    {
        //        _messages.Add(message);
        //    }
        //    finally
        //    {
        //        MessageLock.ExitWriteLock();
        //    }
        //    //新消息信号
        //    SessionWaitHandle.Set();
        //}
        ///// <summary>
        ///// 添加新消息
        ///// </summary>
        //public void HandleMessage(IEnumerable<IMessage> messages)
        //{
        //    MessageLock.EnterWriteLock();
        //    try
        //    {
        //        _messages.AddRange(messages);
        //    }
        //    finally
        //    {
        //        MessageLock.ExitWriteLock();
        //    }
        //    //新消息信号
        //    SessionWaitHandle.Set();
        //}

        /// <summary>
        /// 完成长连接处理
        /// </summary>
        public void FinishRequest(MessageRequest request)
        {
            request.IsTimeOut = (DateTime.Now - request.BeginTime).TotalSeconds >= RequestTimeOut;
            if (request.IsCompleted || request.IsTimeOut)
            {
                DeQueueRequest(request);
                request.FinishRequest();
            }
        }

        /// <summary>
        /// 将请求加入线程处理队列
        /// </summary>
        public void EnQueueRequest(MessageRequest request)
        {
            //将请求加入队列处理
            RequestLock.EnterWriteLock();
            try
            {
                _requestList.AddFirst(request);
            }
            finally
            {
                RequestLock.ExitWriteLock();
            }
            //通知线程开始工作
            ThreadWaitHandle.Set();
        }
        /// <summary>
        /// 完成请求删除节点
        /// </summary>
        public void DeQueueRequest(MessageRequest request)
        {
            RequestLock.EnterWriteLock();
            try
            {
                _requestList.Remove(request);
            }
            finally
            {
                RequestLock.ExitWriteLock();
            }
        }
    }
    public static class MessageThreadPool
    {
        //最大工作线程
        public const int MaxThreadCount = 128;
        //默认工作线程，处理程序为CPU密集操作，默认与cpu核心数相同即可
        public const int DefaultThreadCount = 4;

        private static int _threadCount;
        private static MessageThread[] _threads;
        private static IMessageSessionManager _manager;

        internal static IMessageSessionManager SessionManager
        {
            get { return _manager; }
        }

        /// <summary>
        /// 启动线程池，注册会话处理对象
        /// </summary>
        public static void Start(IMessageSessionManager sessionManager)
        {
            Start(DefaultThreadCount, sessionManager);
        }
        /// <summary>
        /// 启动线程池，注册会话处理对象
        /// </summary>
        public static void Start(int threadCount, IMessageSessionManager sessionManager)
        {
            _manager = sessionManager;
            if (threadCount < MaxThreadCount && threadCount > 0)
                _threadCount = threadCount;
            else
                _threadCount = DefaultThreadCount;

            _threads = new MessageThread[_threadCount];
            for (int i = 0; i < _threadCount; i++)
                _threads[i] = new MessageThread(_manager);
        }
        public static void Stop()
        {
        }

        private static int _index = 0;
        private static object _locker = new object();

        /// <summary>
        /// 把长连接队列
        /// </summary>
        /// <param name="result"></param>
        internal static void QueueRequest(MessageRequest result)
        {
            lock (_locker)
            {
                if (_index >= _threadCount)
                    _index = 0;
                _threads[_index].EnQueueRequest(result);
                ++_index;
            }
        }
    }

#if (DEBUG)
    public sealed class Message : IMessage
    {
        public long Id;
        public string Content;

        public Message(long id, string content)
        {
            Id = id;
            Content = content;
        }
    }
    public sealed class MessageSessionManager : IMessageSessionManager
    {
        private readonly List<IMessage> _messages;
        private ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        public MessageSessionManager()
        {
            _messages = new List<IMessage>();
        }

        public bool DoSession(MessageRequest request)
        {
            try
            {
                if (request.IsPost)
                {
                    _locker.EnterWriteLock();
                    try
                    {
                        Message msg = new Message(request.Id, request.Content);
                        _messages.Add(msg);
                        request.SetMessage(new Message[] { msg });
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                    return true;
                }
                else
                {
                    _locker.EnterWriteLock();
                    try
                    {
                        List<Message> list = new List<Message>(_messages.Count);
                        foreach(Message msg in _messages)
                        {
                            if (msg.Id != request.Id)
                                list.Add(msg);
                        }
                        request.SetMessage(list);
                        foreach (Message msg in list)
                        {
                            if (msg.Id != request.Id)
                                _messages.Remove(msg);
                        }
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
#endif
}
