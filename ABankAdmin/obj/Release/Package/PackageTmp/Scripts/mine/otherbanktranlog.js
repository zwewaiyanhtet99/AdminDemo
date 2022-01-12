//Get checked User ID
//function GetID() {
//    var tr = $("input[name='radio']:checked").parent().parent();
//    if (tr.length == 0) {
//        $("#errMessage").text("Please Choose one user first!");
//        $("#divError").modal('toggle');
//    } else {
//        var id = tr[0].getAttribute('id');
//        return id;
//    }
//}
function GetID() {
    var strID = "";
    $('tbody input[type="checkbox"]:checked').each(function () {
        IDs = $(this).closest('tr').attr('id');
        strID += IDs + ",";
    });
    if (strID.length == 0) {
        $("#errMessage").text("Please Choose one transaction first!");
        $("#divError").modal('toggle');
    } else {
        return strID;
    }
}

//Get user name
function getUsername() {
    var tr = $("input[name='checkbox']:checked").parent().parent();
    return tr;
}

//success user by select id
function success() {
    var id = GetID();
    if (id == null) return;
    var $confirmunLock = $("#modalConfirmYesNo");

    //getting checked tr
    var username = getUsername();
    var statusurl;
    statusurl = "/CCT/SetSuccess";
    $("#hMessage").text("Set Status Confirmation");
    $("#iMessage").text("Are you sure you want to set success status?");

    $confirmunLock.modal('show');
    $('#btnYesConfirmYesNo').on('click', function () {
        //var obj = {
        //    id: parseInt(id)
        //};
        id = GetID();
        //set status pwd
        $.ajax({
            type: "POST",
            url: statusurl,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            //async: false,
            data: JSON.stringify({ id: id }),
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

function fail() {
    var id = GetID();
    if (id == null) return;
    var $confirmunLock = $("#modalConfirmYesNo");

    //getting checked tr
    //var username = getUsername();  
    var statusurl;
    statusurl = "/CCT/SetFail";
    $("#hMessage").text("Set Status Confirmation");
    $("#iMessage").text("Are you sure you want to set fail status ?");
    $confirmunLock.modal('show');
    $('#btnYesConfirmYesNo').on('click', function () {
        //alert(id);
        //var obj = {
        //    id: parseInt(id)
        //};
        id = GetID();
        //set status pwd
        $.ajax({
            type: "POST",
            url: statusurl,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            //async: false,
            data: JSON.stringify({ id: id }),
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

function userPermission() {
    $("#errorMessage").text("You don't have permission !");
    $("#userPermissionError").modal('toggle');
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

function GetBulkID() {
    var tr = $("input[name='checkbox']:checked").parent().parent();
    if (tr.length == 0) {
        $("#errMessage").text("Please choose one first!");
        $("#divError").modal('toggle');
    } else {
        var id = tr[0].getAttribute('id');
        return id;
    }
}
//Bulk Detail
function bulkdetail() {
    var id = GetBulkID();
    if (id == null) return;
    window.location.href = "/CCT/BulkDetail/" + id;
}

//success user by select id
function bulksuccess() {
    var id = GetID();
    if (id == null) return;
    var $confirmunLock = $("#modalConfirmYesNo");

    //getting checked tr
    //var username = getUsername();
    var statusurl;
    statusurl = "/CCT/SetBulkSuccess";
    $("#hMessage").text("Set Status Confirmation");
    $("#iMessage").text("Are you sure you want to set success status?");

    $confirmunLock.modal('show');
    $('#btnYesConfirmYesNo').on('click', function () {
        //var obj = {
        //    id: parseInt(id)
        //};
        id = GetID();
        //set status pwd
        $.ajax({
            type: "POST",
            url: statusurl,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            //async: false,
            data: JSON.stringify({ id: id }),
            cache: false,
            success: function (data) {
                $("#pBulkMessage").text(data);
                $("#divbulkSuccess").modal('toggle');
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

//get username
function getMultiUsername() {
    var usernames = "";
    var tr = $('tbody input[type="checkbox"]:checked').parent().parent();
    for (var i = 0; i < tr.length; i++) {
        usernames += tr[i].children[2].innerText + ",";
    }
    var names = usernames.split(',');
    return names;
};

function getAccountno() {
    var accounts = "";
    var tr = $('tbody input[type="checkbox"]:checked').parent().parent();
    for (var i = 0; i < tr.length; i++) {
        accounts += tr[i].children[1].innerText + ",";
    }
    var acc = accounts.split(',');
    return acc;
};

//bulk fail in pending list
function bulkfail() {
    var id = GetID();
    if (id == null) return;
    var $confirmunLock = $("#modalConfirmYesNo");

    var usernames = getMultiUsername();
    var accounts = getAccountno();
    //getting checked tr
    var statusurl;
    statusurl = "/CCT/SetBulkFail";
    $("#hMessage").text("Set Status Confirmation");
    $("#iMessage").text("Are you sure you want to set fail status ?");
    $('#tranList tr').empty();
    //for table header
    $('#body').append('<tr id="header"><td><b>No.</b></td><td><b>Account Name</b></td><td><b>Account No./NRC</b></td></tr>');

    for (i = 0; i < usernames.length - 1; i++) {
        $('#body').append('<tr id="row"><td>' + (i + 1) + '</td><td>' + usernames[i] + '</td><td>' + accounts[i] + '</td></tr>');
    }

    $confirmunLock.modal('show');
    $('#btnYesConfirmYesNo').on('click', function () {
        id = GetID();
        //alert(id);
        //var obj = {
        //    id: parseInt(id)
        //};
        //set status pwd
        $.ajax({
            type: "POST",
            url: statusurl,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            //async: false,
            data: JSON.stringify({ id: id }),
            cache: false,
            success: function (data) {
                $("#pBulkMessage").text(data);
                $("#divbulkSuccess").modal('toggle');
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

