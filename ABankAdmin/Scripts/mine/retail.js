﻿function check() {
    var obj = getInputData();
    //to check CIFID is null
    if (obj.CIFID == "" || obj.CIFID == undefined) {
        $("#pMessage").text("CIFID is required.");
        $("#divError").modal('toggle');
        return;
    }
    //clear existing searched data
    $("#uservm_CIFID").val('');
    $("#uservm_MOBILENO").val('');
    $("#uservm_FULLNAME").val('');
    $("#uservm_NRC").val('');
    $("#uservm_EMAIL").val('');
    $("#uservm_ADDRESS").val('')
    $("#uservm_MINOR").val('');
    $("#uservm_GENDER").val('');
    $("#uservm_USERNAME").val('');
    $("#accTbody > tr").remove();
    //$("#awttbody > tr").remove();
    //get new by CIFID
    $.ajax({
        type: "GET",
        url: "/User/getRetail",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        data: obj,
        cache: false,
        success: function (data) {
            if (data.NRC != null) {
                $("#uservm_CIFID").val(obj.CIFID);
                $("#uservm_MOBILENO").val(data.PhoneNumber);
                $("#uservm_FULLNAME").val(data.Name);
                $("#uservm_NRC").val(data.NRC);
                $("#uservm_EMAIL").val(data.Email);
                $("#uservm_MINOR").val(data.Minor);
                $("#uservm_GENDER").val(data.Gender);

                //loop all accounts
                if (data.lAcctInfo != undefined && data.lAcctInfo != null) {
                    //acc table
                    var accTable = document.getElementsByTagName('tbody')[0];
                    for (var i = 0; i < data.lAcctInfo.length; i++) {
                        var tr = accTable.insertRow();      // TABLE ROW.
                        //tr.setAttribute('id', 'acc_tr' + i);

                        //Row No
                        var td = document.createElement('td');          // TABLE DEFINITION.
                        td = tr.insertCell(0);
                        var rowno = document.createTextNode(i + 1);
                        td.setAttribute('style', "width:50px;text-align:center;");
                        td.appendChild(rowno);


                        //AccNo column
                        var td = document.createElement('td');
                        td = tr.insertCell(1);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:165px");
                        ele.setAttribute('value', data.lAcctInfo[i]["AccountNumber"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].ACCOUNTNO');
                        ele.setAttribute('id', 'accountvms_' + i + '__ACCOUNTNO');
                        td.appendChild(ele);

                        //Acc Type column
                        var td = document.createElement('td');
                        td = tr.insertCell(2);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:160px");
                        ele.setAttribute('value', data.lAcctInfo[i]["AccountType"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].ACC_TYPE');
                        ele.setAttribute('id', 'accountvms_' + i + '__ACC_TYPE');
                        td.appendChild(ele);

                        //Acc Scheme Code
                        var td = document.createElement('td');
                        td = tr.insertCell(3);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:160px");
                        ele.setAttribute('value', data.lAcctInfo[i]["Schm_Code"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].SCHM_CODE');
                        ele.setAttribute('id', 'accountvms_' + i + 'SCHM_CODE');
                        td.appendChild(ele);

                        //Branch column
                        var td = document.createElement('td');
                        td = tr.insertCell(4);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:180px");
                        ele.setAttribute('value', data.lAcctInfo[i]["BranchID"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].BRANCHCODE');
                        ele.setAttribute('id', 'accountvms_' + i + '__BRANCHCODE');
                        td.appendChild(ele);

                        //Currency column
                        var td = document.createElement('td');
                        td = tr.insertCell(5);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:120px");
                        ele.setAttribute('value', data.lAcctInfo[i]["Currency"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].CURRENCY');
                        ele.setAttribute('id', 'accountvms_' + i + '__CURRENCY');
                        td.appendChild(ele);

                        //checkbox column
                        var td = document.createElement('td');          // TABLE DEFINITION.
                        td = tr.insertCell(6);
                        var cbox = document.createElement('input');
                        cbox.setAttribute('type', 'checkbox');// SET INPUT ATTRIBUTE.
                        cbox.setAttribute('value', true);
                        td.setAttribute('style', "width:80px;text-align:center;");
                        cbox.setAttribute('name', 'accountvms[' + i + '].Active');
                        cbox.setAttribute('id', 'accountvms_' + i + '__Active');
                        //check change event
                        cbox.setAttribute('onclick', 'checkChange(' + i + ')');
                        td.appendChild(cbox);

                        //QR Allow
                        var td = document.createElement('td');
                        td = tr.insertCell(7);
                        var cbox = document.createElement('input');
                        cbox.setAttribute('type', 'checkbox');// SET INPUT ATTRIBUTE.
                        cbox.setAttribute('value', true);
                        td.setAttribute('style', "width:90px;text-align:center;");
                        //QR Allow disable if Acc type is ODA, LAA or TDA
                        //var AccType = data.lAcctInfo[i]["AccountType"];
                        //if (AccType == "ODA" || AccType == "LAA" || AccType == "TDA") {
                        //    cbox.setAttribute('disabled', 'disabled');
                        //}
                        cbox.setAttribute('disabled', 'disabled');
                        cbox.setAttribute('name', 'accountvms[' + i + '].QR_ALLOW');
                        cbox.setAttribute('id', 'accountvms_' + i + '__QR_ALLOW');
                        td.appendChild(cbox);

                        //Acc Type column
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'hidden');
                        ele.setAttribute('value', data.lAcctInfo[i]["AccountTypeDesc"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].ACC_DESC');
                        ele.setAttribute('id', 'accountvms_' + i + '__ACC_DESC');
                        td.appendChild(ele);
                    }
                }

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
            console.log(xhr.responseText);
        }
    });
    return;
}

function getInputData() {
    return { CIFID: $("#uservm_CIFID").val().trim() };
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
    var chk= $(lock).is(":checked")
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

//set signin pwd
//function setsigninpwd()
//{
//    var radioValue = $("input[name='user']:checked").val();
//    if (radioValue == null) {
//        $("#errMessage").text("Please Choose one user first!");
//        $("#divError").modal('toggle');
//    }
//    else {
//        //alert(radioValue);
//        var obj = {
//            id: parseInt(radioValue)
//        };
//        //set signin pwd
//        $.ajax({
//            type: "POST",
//            url: "/User/SetSigninPwd",
//            contentType: 'application/json; charset=utf-8',
//            dataType: 'json',
//            async: false,
//            data: JSON.stringify(obj),
//            cache: false,
//            success: function (data) {
//                $("#pMessage").text(data.msg);
//                $("#divSuccess").modal('toggle');
//                //download pdf
//                download(data.filename);
//            },
//            error: function (xhr, status, error) {
//                alert(xht.status);
//            }
//        });
//    }
//}

//set signin pwd
//function settranpwd() {
//    var radioValue = $("input[name='user']:checked").val();
//    if (radioValue == null) {
//        $("#errMessage").text("Please Choose one user first!");
//        $("#divError").modal('toggle');
//    }
//    else {
//        //alert(radioValue);
//        var obj = {
//            id: parseInt(radioValue)
//        };
//        //set signin pwd
//        $.ajax({
//            type: "POST",
//            url: "/User/SetTranPwd",
//            contentType: 'application/json; charset=utf-8',
//            dataType: 'json',
//            //async: false,
//            data: JSON.stringify(obj),
//            cache: false,
//            success: function (data) {
//                $("#pMessage").text(data.msg);
//                $("#divSuccess").modal('toggle');
//                //download pdf
//                download(data.filename);
//            },
//            error: function (xhr, status, error) {
//                alert(xhr.responseText);
//            }
//        });
//    }
//}

//set signin pwd

//function lock() {
//    var id = GetID();
//    if (id == null) return;//to exit
//        var $confirmLock = $("#modalConfirmYesNo");

//        //getting checked tr
//        var username = getUsername();
//        $("#hMessage").text("Lock user Confirmation");
//        $("#iMessage").text("Are you sure you want to lock for username \"" + username + "\" ?");
//        $confirmLock.modal('show');
//        $('#btnYesConfirmYesNo').on('click', function () {
//        //alert(id);
//       var obj = {
//            id: parseInt(id)
//        };
//        //set signin pwd
//            $.ajax({
//                type: "POST",
//                url: "/User/Lock",
//                contentType: 'application/json; charset=utf-8',
//                dataType: 'json',
//                async: false,
//                data: JSON.stringify(obj),
//                cache: false,
//                success: function (data) {
//                    $("#pMessage").text(data);
//                    $("#divSuccess").modal('toggle');
//                },
//                error: function (xhr, status, error) {
//                    alert(xhr.responseText);
//                }
//            });
//            $confirmLock.modal("hide");
//        });
//        $("#btnNoConfirmYesNo").off('click').click(function () {
//            $('#modalConfirmYesNo').modal('hide');
//        });
//    }
//set signin pwd
//function unlock() {
//    var id = GetID();
//    if (id == null) return;//to exit
//    var $confirmunLock = $("#modalConfirmYesNo");

//    //getting checked tr
//    var username = getUsername();
//    $("#hMessage").text("Unlock user Confirmation");
//    $("#iMessage").text("Are you sure you want to unlock for username \"" + username + "\" ?");
//    $confirmunLock.modal('show');
//    $('#btnYesConfirmYesNo').on('click', function () {
//        //alert(id);
//        var obj = {
//            id: parseInt(id)
//        };
//        //set signin pwd
//        $.ajax({
//            type: "POST",
//            url: "/User/Unlock",
//            contentType: 'application/json; charset=utf-8',
//            dataType: 'json',
//            //async: false,
//            data: JSON.stringify(obj),
//            cache: false,
//            success: function (data) {
//                $("#pMessage").text(data);
//                $("#divSuccess").modal('toggle');
//            },
//            error: function (xhr, status, error) {
//                alert(xhr.responseText);
//            }
//        });
//        $confirmunLock.modal("hide");
//    });
//    $("#btnNoConfirmYesNo").off('click').click(function () {
//        $('#modalConfirmYesNo').modal('hide');
//    });
//}
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
    else
    {
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
//set tranlock
//function tranlock() {
//    var id = GetID();
//    if (id == null) return;//to exit
//        var $confirmLock = $("#modalConfirmYesNo");

//        //getting checked tr
//        var username = getUsername();
//        $("#hMessage").text("Lock user Confirmation");
//        $("#iMessage").text("Are you sure you want to lock for username \"" + username + "\" ?");
//        $confirmLock.modal('show');
//        $('#btnYesConfirmYesNo').on('click', function () {
//        //alert(id);
//       var obj = {
//            id: parseInt(id)
//        };
//        //set signin pwd
//            $.ajax({
//                type: "POST",
//                url: "/User/TransactionLock",
//                contentType: 'application/json; charset=utf-8',
//                dataType: 'json',
//                async: false,
//                data: JSON.stringify(obj),
//                cache: false,
//                success: function (data) {
//                    $("#pMessage").text(data);
//                    $("#divSuccess").modal('toggle');
//                },
//                error: function (xhr, status, error) {
//                    alert(xhr.responseText);
//                }
//            });
//            $confirmLock.modal("hide");
//        });
//        $("#btnNoConfirmYesNo").off('click').click(function () {
//            $('#modalConfirmYesNo').modal('hide');
//        });
//    }



////set signin tranunlock
//function tranunlock() {
//    var id = GetID();
//    if (id == null) return;//to exit
//        var $confirmunLock = $("#modalConfirmYesNo");

//        //getting checked tr
//        var username = getUsername();
//        $("#hMessage").text("Unlock user Confirmation");
//        $("#iMessage").text("Are you sure you want to unlock for username \"" + username + "\" ?");
//        $confirmunLock.modal('show');
//        $('#btnYesConfirmYesNo').on('click', function () {
//            //alert(id);
//            var obj = {
//                id: parseInt(id)
//            };
//            //set signin pwd
//            $.ajax({
//                type: "POST",
//                url: "/User/TransactionUnlock",
//                contentType: 'application/json; charset=utf-8',
//                dataType: 'json',
//                //async: false,
//                data: JSON.stringify(obj),
//                cache: false,
//                success: function (data) {
//                    $("#pMessage").text(data);
//                    $("#divSuccess").modal('toggle');
//                },
//                error: function (xhr, status, error) {
//                    alert(xhr.responseText);
//                }
//            });
//            $confirmunLock.modal("hide");
//        });
//        $("#btnNoConfirmYesNo").off('click').click(function () {
//            $('#modalConfirmYesNo').modal('hide');
//        });
//}

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

////VIP check change
//$("#uservm_IsVIP").change(function () {
//    if ($("#uservm_IsVIP")[0].checked) {
//        //$(".OnlyVIP").show();
//        //allow editing
//        $("#uservm_ALLOW_TRAN_COUNT")[0].removeAttribute('readonly');
//        $("#uservm_DAILY_MAX_AMT")[0].removeAttribute('readonly');
//    } else {
//        //$(".OnlyVIP").hide();
//        //set Rule value and make read-only
//        console.log(ViewBag.a);
//        $("#uservm_ALLOW_TRAN_COUNT")[0].setAttribute('readonly', 'readonly');
//        $("#uservm_DAILY_MAX_AMT")[0].setAttribute('readonly', 'readonly');
//    }
//});
