using System;
using Cnaws.Web;
using Cnaws.Data;
using P = Cnaws.Product.Logistics;

namespace Cnaws.Product.Controllers
{
    public sealed class Logistics : DataController
    {
        public void Providers(string type = "json")
        {
            if ("json".Equals(type, StringComparison.OrdinalIgnoreCase))
                SetResult(true, P.LogisticsProvider.Providers);
            else
                SetJavascript("allLogistics", P.LogisticsProvider.Providers);
        }

        public void Search(string provider, string order)
        {
            try
            {
                SetResult(true, P.LogisticsProvider.Create(provider).Search(order));
            }
            catch (Exception ex)
            {
                SetResult(false, ex.Message);
            }
        }

#if (DEBUG)
        public void Debug()
        {
            Response.Write(string.Concat(@"<!doctype html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>物流查询</title>
    <script type=""text/javascript"" src=""", GetRes("/resource/static/scripts/jquery-1.11.1.min.js"), @"""></script>
    <script type=""text/javascript"" src=""", GetRes("/resource/static/scripts/cnaws.js"), @"""></script>
    <script type=""text/javascript"">
        $(document).ready(function () {
            Cnaws.Init({
                urlExt: '", this["ext"], @"',
                resourcesUrl: '", Application.Settings.ResourcesUrl, @"'
            });
            Cnaws.getAjax('", GetUrl("/logistics/providers/json"), @"', function (data, args) {
                if (data.code == -200) {
                    var html = '';
                    for (var i = 0; i < data.data.length; ++i) {
                        html += '<option value=""' + data.data[i].Key + '"">' + data.data[i].Name + '</option>';
                    }
                    $('#type').html(html);
                }
            }, null);
        });
        function onSearch() {
            $('#info').html('');
            var form = $('#form');
            form.attr('disabled', true);
            Cnaws.getAjax(Cnaws.getUrl('", GetUrl("/logistics/search/"), @"' + $('#type').val() + '/' + $('#q').val()), function (data, args) {
                if (data.code == -200) {
                    var html = '';
                    for (var i = 0; i < data.data.length; ++i) {
                        html += '<li><dl><dt>' + data.data[i].Time + '</dt><dd>' + data.data[i].Status + '</dd></dl></li>';
                    }
                    $('#info').html(html);
                }
                else {
                    $('#info').html(data.data);
                }
                args.form.attr('disabled', false);
            }, { form: form });
            return false;
        }
    </script>
</head>
<body>
    <form id=""form"" action="""" method=""get"" onsubmit=""return onSearch()"">
        <select id=""type"" name=""type""></select>
        <input id=""q"" name=""q"" type=""text""/>
        <input type=""submit"" value=""查询""/>
    </form>
    <ul id=""info""></ul>
</body>
</html>"));
        }
#endif
    }
}
