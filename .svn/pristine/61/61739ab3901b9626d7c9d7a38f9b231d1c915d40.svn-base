//Get check user id
//function GetID() {
//    var id = $("input[name='admin']:checked").val();
//    if (id == null) {
//        $("#errMessage").text("Please Choose one Admin first!");
//        $("#divError").modal('toggle');
//    } else {
//        return id;
//    }
//}

function GetUserID() {
    var tr = $("input[name='radio']:checked").parent().parent();
    if (tr.length == 0) {
        $("#errMessage").text("Please Choose one Admin first!");
        $("#divError").modal('toggle');
    } else {
        var id = tr[0].getAttribute('id');   
        return id;
    }
}

//Get check user name
function getAdminname() {
    var tr = $("input[name='radio']:checked").parent().parent();
    var adminname = tr[0].children[1].innerText;
    if (adminname != null) { return adminname;}
}

//Set Reset pwd
function resetpwd() {
    //var id = GetID();
    var id = GetUserID();
        var $confirm = $("#modalConfirmYesNo");

    //getting checked tr
    var admin = getAdminname();
        $("#hMessage").text("Reset Password Confirmation");
        $("#iMessage").text("Are you sure you want to reset password for admin \"" + admin + "\" ?");
        $confirm.modal('show');
        $('#btnYesConfirmYesNo').on('click', function () {
        var obj = {
            id: id
        };
        //set signin pwd
        $.ajax({
            type: "POST",
            url: "/Admin/ResetPwd",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(obj),
            cache: false,
            success: function (data) {
                $("#pMessage").text(data.msg);
                $("#divSuccess").modal('toggle');
                //download pdf
                download(data.filename);
            },
            error: function (xhr, status, error) {
                if (status != null) {
                    adminPermission();
                }
                //console.log(xhr.responseText);
            }
        });
            $confirm.modal("hide");
        });

        $("#btnNoConfirmYesNo").off('click').click(function () {
            $('#modalConfirmYesNo').modal('hide');
        });
    }

function lock() {
    var id = GetUserID();
        var $confirmLock = $("#modalConfirmYesNo");

    //getting checked tr
    var admin = getAdminname();
        $("#hMessage").text("Lock admin Confirmation");
        $("#iMessage").text("Are you sure you want to lock for admin \"" + admin + "\" ?");
        $confirmLock.modal('show');
        $('#btnYesConfirmYesNo').on('click', function () {
        var obj = {
            id: id
        };
        //set signin pwd
        $.ajax({
            type: "POST",
            url: "/Admin/Lock",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(obj),
            cache: false,
            success: function (data) {
                $("#pMessage").text(data);
                $("#divSuccess").modal('toggle');
            },
            error: function (xhr, status, error) {
                if (status != null) {
                    adminPermission();
                }
                //console.log(xhr.responseText);
            }
        });
            $confirmLock.modal("hide");
        });
        $("#btnNoConfirmYesNo").off('click').click(function () {
            $('#modalConfirmYesNo').modal('hide');
        });
    }

//set signin pwd
function unlock() {
    var id = GetUserID();
        var $confirmunLock = $("#modalConfirmYesNo");

    //getting checked tr
    var admin = getAdminname();
        $("#hMessage").text("Unlock admin Confirmation");
        $("#iMessage").text("Are you sure you want to unlock for admin \"" + admin + "\" ?");
        $confirmunLock.modal('show');
        $('#btnYesConfirmYesNo').on('click', function () {
        //alert(radioValue);
        var obj = {
            id: id
        };
        //set signin pwd
        $.ajax({
            type: "POST",
            url: "/Admin/Unlock",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            //async: false,
            data: JSON.stringify(obj),
            cache: false,
            success: function (data) {
                $("#pMessage").text(data);
                $("#divSuccess").modal('toggle');
            },
            error: function (xhr, status, error) {
                if (status != null) {
                    adminPermission();
                }
                //console.log(xhr.responseText);
            }
        });
            $confirmunLock.modal("hide");
        });
        $("#btnNoConfirmYesNo").off('click').click(function () {
            $('#modalConfirmYesNo').modal('hide');
        });
}

//allow check change


function download(name) {
    window.location.href = "/Admin/Download/?filename=" + name;
}

//edit user by select id
function edit() {
    var id = GetUserID();
    if (id != null) {
        window.location.href = "/Admin/Edit/" + id;
    }
}

//delete user data by id
function Delete() {
    var id = GetUserID();
    if (id != null) {
        window.location.href = "/Admin/Delete/" + id;
    }
}

function adminPermission() {
    $("#errorMessage").text("You don't have permission !");
    $("#adminPermissionError").modal('toggle');
}