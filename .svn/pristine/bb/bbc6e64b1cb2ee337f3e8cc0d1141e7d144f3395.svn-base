function check() {
    var obj = getInputData();
    //to check CIFID is null
    if (obj.CIFID == "" || obj.CIFID == undefined) {
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
        url: "/Request/getRetail",
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
                        td.setAttribute('style', "width:150px");
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
                        td.setAttribute('style', "width:170px");
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
                        td.setAttribute('style', "width:110px");
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
                        td.setAttribute('style', "width:75px;text-align:center;");
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
            else if (data.ResponseCode != '000') {
                $("#pMessage").text(data.ResponseDesc);
                $("#divError").modal('toggle');
            }
            else {
                $("#pMessage").text("This CIFID doesn't exist.");
                $("#divError").modal('toggle');
            }
        },
        error: function (xhr, status, error) {
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

//Get user name
function GetUsername() {
    var tr = $("input[name='radio']:checked").parent().parent();
    var username = tr[0].children[2].innerText;
    if (username != null) { return username; }
}

//Set Reset pwd
function resetpwd() {
    var id = GetID();
    if (id == null) return; //to exit
    var $confirm = $("#modalConfirmYesNo");

    var username = GetUsername();//get name
    $("#hMessage").text("Reset Password Confirmation");
    $("#iMessage").text("Are you sure you want to Request to reset password for username \"" + username + "\" ?");
    $confirm.modal('show');
    $('#btnYesConfirmYesNo').on('click', function () {
        var obj = {
            id: parseInt(id)
        };
        //set signin pwd
        $.ajax({
            type: "POST",
            url: "/request/reset",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            data: JSON.stringify(obj),
            cache: false,
            success: function (data) {
                $("#pMessage").text(data);
                $("#divSuccess").modal('toggle');
                //check filename exist and call  download() function
            },
            error: function (xhr, status, error) {
                if (status != null) { //permission error message
                    reqPermission();
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

function lock_unlock() {
    var id = GetID();
    if (id == null) return;//to exit
    var $confirmunLock = $("#modalConfirmYesNo");

    //getting checked tr
    var username = getUsername();
    var lock = getlock();
    var lockurl;
    if (lock == true) {
        lockurl = "/request/Unlock";
        $("#hMessage").text("Unlock user Confirmation");
        $("#iMessage").text("Are you sure you want to unlock for username \"" + username + "\" ?");
    }
    else {
        lockurl = "/request/Lock";
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
            url: lockurl,
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
                if (status != null) { //permission error message
                    reqPermission();
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
        tranurl = "/Request/TranUnlock";
        $("#hMessage").text("Transaction Unlock user Confirmation");
        $("#iMessage").text("Are you sure you want to unlock for username \"" + username + "\" ?");
    }
    else {
        tranurl = "/Request/TranLock";
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
                if (status != null) { //permission error message
                    reqPermission();
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

//function lock() {
//    var radioValue = $("input[name='user']:checked").val();
//    if (radioValue == null) {
//        $("#errMessage").text("Please Choose one user first!");
//        $("#divError").modal('toggle');
//    }
//    else {
//        var $confirmLock = $("#modalConfirmYesNo");

//        //getting checked tr
//        var tr = $("input[name='user']:checked").parent().parent();
//        var username = tr[0].children[2].innerText;
//        $("#hMessage").text("Lock user Confirmation");
//        $("#iMessage").text("Are you sure you want to lock for username \"" + username + "\" ?");
//        $confirmLock.modal('show');
//        $('#btnYesConfirmYesNo').on('click', function () {
//            //alert(radioValue);
//            var obj = {
//                id: parseInt(radioValue)
//            };
//            //set signin pwd
//            $.ajax({
//                type: "POST",
//                url: "/request/Lock",
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
//                    console.log(xhr.responseText);
//                }
//            });
//            $confirmLock.modal("hide");
//        });
//        $("#btnNoConfirmYesNo").off('click').click(function () {
//            $('#modalConfirmYesNo').modal('hide');
//        });
//    }
//}

////set tranlock
//function tranlock() {
//    var radioValue = $("input[name='user']:checked").val();
//    if (radioValue == null) {
//        $("#errMessage").text("Please Choose one user first!");
//        $("#divError").modal('toggle');
//    }
//    else {
//        var $confirmLock = $("#modalConfirmYesNo");

//        //getting checked tr
//        var tr = $("input[name='user']:checked").parent().parent();
//        var username = tr[0].children[2].innerText;
//        $("#hMessage").text("Lock user Confirmation");
//        $("#iMessage").text("Are you sure you want to lock for username \"" + username + "\" ?");
//        $confirmLock.modal('show');
//        $('#btnYesConfirmYesNo').on('click', function () {
//            //alert(radioValue);
//            var obj = {
//                id: parseInt(radioValue)
//            };
//            //set signin pwd
//            $.ajax({
//                type: "POST",
//                url: "/Request/TranLock",
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
//                    console.log(xhr.responseText);
//                }
//            });
//            $confirmLock.modal("hide");
//        });
//        $("#btnNoConfirmYesNo").off('click').click(function () {
//            $('#modalConfirmYesNo').modal('hide');
//        });
//    }
//}

////set signin pwd
//function unlock() {
//    var radioValue = $("input[name='user']:checked").val();
//    if (radioValue == null) {
//        $("#errMessage").text("Please Choose one user first!");
//        $("#divError").modal('toggle');
//    }
//    else {
//        var $confirmunLock = $("#modalConfirmYesNo");

//        //getting checked tr
//        var tr = $("input[name='user']:checked").parent().parent();
//        var username = tr[0].children[2].innerText;
//        $("#hMessage").text("Unlock user Confirmation");
//        $("#iMessage").text("Are you sure you want to unlock for username \"" + username + "\" ?");
//        $confirmunLock.modal('show');
//        $('#btnYesConfirmYesNo').on('click', function () {
//            //alert(radioValue);
//            var obj = {
//                id: parseInt(radioValue)
//            };
//            //set signin pwd
//            $.ajax({
//                type: "POST",
//                url: "/request/Unlock",
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
//                    console.log(xhr.responseText);
//                }
//            });
//            $confirmunLock.modal("hide");
//        });
//        $("#btnNoConfirmYesNo").off('click').click(function () {
//            $('#modalConfirmYesNo').modal('hide');
//        });
//    }
//}

////set signin tranunlock
//function tranunlock() {
//    var radioValue = $("input[name='user']:checked").val();
//    if (radioValue == null) {
//        $("#errMessage").text("Please Choose one user first!");
//        $("#divError").modal('toggle');
//    }
//    else {
//        var $confirmunLock = $("#modalConfirmYesNo");

//        //getting checked tr
//        var tr = $("input[name='user']:checked").parent().parent();
//        var username = tr[0].children[2].innerText;
//        $("#hMessage").text("Unlock user Confirmation");
//        $("#iMessage").text("Are you sure you want to unlock for username \"" + username + "\" ?");
//        $confirmunLock.modal('show');
//        $('#btnYesConfirmYesNo').on('click', function () {
//            //alert(radioValue);
//            var obj = {
//                id: parseInt(radioValue)
//            };
//            //set signin pwd
//            $.ajax({
//                type: "POST",
//                url: "/Request/TranUnlock",
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
//                    console.log(xhr.responseText);
//                }
//            });
//            $confirmunLock.modal("hide");
//        });
//        $("#btnNoConfirmYesNo").off('click').click(function () {
//            $('#modalConfirmYesNo').modal('hide');
//        });
//    }
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
    if (id == null) return;
    window.location.href = "/request/edit/" + id;
}

//check user data details by id
//function detail() {
//    var radioValue = $("input[name='user']:checked").val();
//    if (radioValue == null) {
//        $("#errMessage").text("Please Choose one user first!");
//        $("#divError").modal('toggle');
//    } else {
//        window.location.href = "/Request/Detail/" + radioValue;
//    }
//} 

//check user data details by id
function detail() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/Request/Detail/" + id;
}

function useredit() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/request/UserEdit/" + id;
}

function userdetail() {
    var id = GetID();
    if (id == null) return;    
    window.location.href = "/request/UserDetails/" + id;    
} 

function userdelete() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/request/UserDelete/" + id;    
}

function UserDeleteConfirmed(id) {
    var obj = {
        id: parseInt(id)
    };
    if (id == null) return;             
        $.ajax({
            type: "POST",
            url: "/request/UserDelete",
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
               console.log(xhr.responseText);
            }
        });
    }

//delete user data by id
function deletereq() {
    var id = GetID();
    if (id == null) return;   
     window.location.href = "/Request/Delete/" + id;
   
}

//permission error message
function reqPermission() {
    $("#errorMessage").text("You don't have permission !");
    $("#reqPermissionError").modal('toggle');
}

//start date and end date validation
function dateValidation() {
        var start = $('#FromDate').val().split("-");
        var from = new Date(start[2], start[1] - 1, start[0])//Date.parse(start);
        var end = $('#ToDate').val().split("-");
        var to = new Date(end[2], end[1] - 1, end[0])// Date.parse(end);
        if (from > to) {
            $("#errorMessage").text("To Date must be greater than From Date!");
            $("#divDateValidate").modal('toggle');
        }
}


