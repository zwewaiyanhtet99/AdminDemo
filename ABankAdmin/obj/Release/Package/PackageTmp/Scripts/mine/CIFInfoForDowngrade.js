var getusertype = "";
function checkInfo() {
    var obj = getInputData();
    //to check CIFID is null
    if (obj.CIFID == "" || obj.CIFID == undefined) {
        //to check iconic number is null
        if (obj.IconicNo == "" || obj.IconicNo == undefined) {
            $("#pMessage").text("CIFID/Iconic is required.");
            $("#divError").modal('toggle');
            return;
        }
    }


    //clear existing searched data
    $("#CIFID").val('');
    $("#PHONENO").val('');
    $("#NAME").val('');
    $("#NRC").val('');
    $("#USERTYPE").val('');
    $("#USERTYPECODE").val('');
    $("#ADDRESS").val('');
    $("#strEXPIREDATE").val('');
    $("#REMARK").val('');
    $("#accTbody > tr").remove();

    //$("#awttbody > tr").remove();
    //get new by CIFID
    $.ajax({
        type: "GET",
        url: "/CIFInfoDowngrade/getCIFIDInfoForDowngrade",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        data: obj,
        cache: false,
        success: function (data) {
            if (data.NRC != null) {

                $("#CIFID").val(data.CIFID);
                $("#PHONENO").val(data.PHONENO);
                $("#NAME").val(data.NAME);
                $("#NRC").val(data.NRC);
                $("#Address").val(data.Address);
                $("#USERTYPECODE").val(data.USERTYPECODE);
                $("#USERTYPE").val(data.USERTYPE);
                $("#BranchName").val(data.BranchName);
                $("#strEXPIREDATE").val(data.strEXPIREDATE);
                $("#strEffectiveDate").val(data.strEffectiveDate);
                $("#RMName").val(data.RMName);
                $("#RMID").val(data.RMID);
                $("#ID").val(data.ID);
                getUsertypeData($("#USERTYPE").val());
                return;
            }
            else if (data.message != null) {
                $("#pMessage").text(data.message);
                $("#divError").modal('toggle');
            }
            else if (data.ResponseCode != null & data.ResponseCode != '000') {
                $("#pMessage").text(data.ResponseDesc);
                $("#divError").modal('toggle');
            }
            else {
                $("#pMessage").text("This CIFID doesn't exist.");
                $("#divError").modal('toggle');
            }
        },
        error: function (xhr, status, error) {
            if (status != null) {
                userPermission();
            }

        }
    });
    return;
}

function getInputData() {
    return { CIFID: $("#CIFID").val().trim(), IconicNo: $("#USERTYPECODE").val().trim() };
}

function getUsertypeData(usertype) {
    if (usertype != null || usertype !== "") {
        $.ajax({
            type: 'POST',
            url: "/CIFInfoDowngrade/GetBookingListByIconicTypeForDegrade",
            dataType: 'json',
            data: { iconic: usertype },
            success: function (state) {

                $.each(state, function (i, state) {
                    $("#IconicReservedlist").empty();
                    $("#IconicReservedlist").append('<option value="'
                        + state.Value + '">'
                        + state.Text + '</option>');
                });
            },
            error: function (xhr, ajaxError, thrown) {
                if (xhr.status == 302) {
                    window.location.href = "/Admin/Login";
                }
            }
        });
    }
    return;
}

//removing accouts if form cancel
function cancel() {
    var form = document.getElementsByTagName('form')[0];
    form.reset();
    $("#accTbody > tr").remove();
}

//Get checked User ID
function GetID() {
    var tr = $("input[name='radio']:checked").parent().parent();
    if (tr.length == 0) {
        $("#errMessage").text("Please Choose one user first!");
        $("#divError").modal('toggle');
    } else {
        var id = tr[0].getAttribute('id');
        return id;
    }
}

//Get user name
function getUsername() {
    var tr = $("input[name='radio']:checked").parent().parent();
    var username = tr[0].children[2].innerText;
    if (username != null) { return username; }
}
function getlock() {
    var tr = $("input[name='radio']:checked").parent().parent();
    var lock = tr[0].children[7].innerHTML;
    var chk = $(lock).is(":checked")
    if (chk != null) {
        return chk;
    }
}
function gettranlock() {
    var tr = $("input[name='radio']:checked").parent().parent();
    var lock = tr[0].children[8].innerHTML;
    var chk = $(lock).is(":checked")
    if (chk != null) {
        return chk;
    }
}
//function getTranlock() {
//    var tr = $("input[name='user']:checked").parent().parent();
//    var username = tr[0].children[2].innerText;
//    if (username != null) { return username; }
//}

//Set Reset pwd
function resetpwd() {
    var id = GetID();
    if (id == null) return;//to exit
    var $confirm = $("#modalConfirmYesNo");

    //getting checked tr

    var username = getUsername();
    $("#hMessage").text("Reset Password Confirmation");
    $("#iMessage").text("Are you sure you want to reset password for username \"" + username + "\" ?");
    $confirm.modal('show');
    $('#btnYesConfirmYesNo').on('click', function () {
        var obj = {
            id: parseInt(id)
        };
        //set signin pwd
        $.ajax({
            type: "POST",
            url: "/User/SetResetPwd",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
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
                    userPermission();
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


//set singin lock/unlock
//set signin pwd
function signlock_unlock() {
    var id = GetID();
    if (id == null) return;//to exit
    var $confirmunLock = $("#modalConfirmYesNo");

    //getting checked tr
    var username = getUsername();
    var lock = getlock();
    var signinurl;
    if (lock == true) {
        signinurl = "/User/Unlock";
        $("#hMessage").text("Unlock user Confirmation");
        $("#iMessage").text("Are you sure you want to unlock for username \"" + username + "\" ?");
    }
    else {
        signinurl = "/User/Lock";
        $("#hMessage").text("Lock user Confirmation");
        $("#iMessage").text("Are you sure you want to lock for username \"" + username + "\" ?");
    }

    $confirmunLock.modal('show');
    $('#btnYesConfirmYesNo').on('click', function () {
        //alert(id);
        var obj = {
            id: parseInt(id)
        };
        //set signin pwd
        $.ajax({
            type: "POST",
            url: signinurl,
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
                    userPermission();
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

function tranlock_unlock() {
    var id = GetID();
    if (id == null) return;//to exit
    var $confirmunLock = $("#modalConfirmYesNo");

    //getting checked tr
    var username = getUsername();
    var tranlock = gettranlock();
    var tranurl;
    if (tranlock == true) {
        tranurl = "/User/TransactionUnlock";
        $("#hMessage").text("Transaction Unlock user Confirmation");
        $("#iMessage").text("Are you sure you want to unlock for username \"" + username + "\" ?");
    }
    else {
        tranurl = "/User/TransactionLock";
        $("#hMessage").text("Transaction Lock user Confirmation");
        $("#iMessage").text("Are you sure you want to lock for username \"" + username + "\" ?");
    }

    $confirmunLock.modal('show');
    $('#btnYesConfirmYesNo').on('click', function () {
        //alert(id);
        var obj = {
            id: parseInt(id)
        };
        //set signin pwd
        $.ajax({
            type: "POST",
            url: tranurl,
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
                    userPermission();
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
function checkChange(row) {
    var cbox = $('#accountvms_' + row + '__Active')[0];
    var acctype = $('#accountvms_' + row + '__ACC_TYPE')[0];
    var acccurrency = $('#accountvms_' + row + '__CURRENCY')[0];
    acctype = acctype.value;
    acccurrency = acccurrency.value;
    var isAllowed = (acctype != 'ODA' && acctype != 'LAA' && acctype !== 'TDA' /*&& acccurrency == 'MMK'*/);
    var qrcbox = $('#accountvms_' + row + '__QR_ALLOW')[0];
    if (cbox.checked == true & isAllowed) {
        qrcbox.removeAttribute('disabled');
    } else {
        qrcbox.checked = false;
        qrcbox.setAttribute('disabled', 'disabled');
    }
}

function download(name) {
    window.location.href = "/User/Download/?filename=" + name;
}

//edit user by select id
function edit() {
    var id = GetID();
    if (id != null) {
        window.location.href = "/User/Edit/" + id;
    }
}

//check user data details by id
function detail() {
    var id = GetID();
    if (id != null) {
        window.location.href = "/User/Details/" + id;
    }
}

//delete user data by id
function Delete() {
    var id = GetID();
    if (id != null) {
        window.location.href = "/User/Delete/" + id;
    }
}

function userPermission() {
    $("#errorMessage").text("You don't have permission !");
    $("#userPermissionError").modal('toggle');
}




