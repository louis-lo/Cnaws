$('#dlgLogin').jqxWindow({ autoOpen: false, isModal: true, draggable: false, height: 'auto', width: 'auto', theme: 'dialog' });
$("#login_username").jqxInput({ placeHolder: "如name@example.com", minLength: 1 });
$("#login_password").jqxInput({ placeHolder: "请输入密码", minLength: 1 });
function showLogin() {
    $('#dlgLogin').jqxWindow('open');
}