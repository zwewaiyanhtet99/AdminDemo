function check(){
    var obj = getInputData();
    //to check CIFID is null
    if (obj.CIFID == "" || obj.CIFID == undefined) {
        $("#pMessage").text("CIFID is required.");
        $("#divError").modal('toggle');
        return;
    }
    //clear existing searched data
    $("#CIFID").val('');
    $("#Bank_ID").val('');
    $("#Corporate_ID").val('');
    $("#Company_Name").val('');
    $("#Company_Email").val('');
    $("#Company_Address").val('');
    $("#Company_Phone").val('');
    $("#Branch").val('');
    $("#State").val('');
    //$("#Tran_Limit").val('');
    //$("#Bulk_Charges_Fix_Rate").val('');
    //$("#accTbody > tr").remove();
    //$("#awttbody > tr").remove();
    //get new by CIFID
    $.ajax({
        type: "GET",
        url: "/Corporate/getCorporate",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        data: obj,
        cache: false,
        success: function (data) {
            if (data.Name != null) {
                $("#CIFID").val(obj.CIFID);
                $("#Company_Name").val(data.Name);
                $("#Company_Email").val(data.Email);
                //$("#Company_Address").val(data.Minor);
                //$("#Company_Phone").val(data.PhoneNumber);            
                console.log();
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
            console.log(xhr.responseText);
        }
    });
    return;
}

function getInputData() {
    return { CIFID: $("#CIFID").val().trim() };
}

//removing accouts if form cancel
function cancel() {
    var form = document.getElementsByTagName('form')[0];
    form.reset();
   // $("#accTbody > tr").remove();
}

//Get checked User ID
function GetID() {
    var tr = $("input[name='radio']:checked").parent().parent();
    if (tr.length == 0) {
        $("#errMessage").text("Please choose one company first!");
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
    window.location.href = "/Corporate/edit/" + id;
}

//Corporate Request Detail
function detail() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/Corporate/Details/" + id;
}
//delete user data by id
function deletereq() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/Corporate/Delete/" + id;

}
//edit Corporate List
function corlstedit() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/Corporate/CorporateEdit/" + id;
}
//Corporate List Detail
function corlstdetail() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/Corporate/CorporateDetail/" + id;
}
//Corporate List Delete
function corlstdelete() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/Corporate/CorporateDelete/" + id;
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

function CorlstDeleteConfirmed(id) {
    var obj = {
        id: parseInt(id)
    };
    if (id == null) return;
    $.ajax({
        type: "POST",
        url: "/Corporate/CorporateDelete",
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


