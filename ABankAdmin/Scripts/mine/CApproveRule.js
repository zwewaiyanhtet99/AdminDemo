//Get checked User ID
function GetID() {
    var tr = $("input[name='radio']:checked").parent().parent();
    if (tr.length == 0) {
        $("#errMessage").text("Please choose one row first!");
        $("#divError").modal('toggle');
    } else {
        var id = tr[0].getAttribute('id');
        return id;
    }
}

//approve
function approve() {
    var id = GetID();
    if (id == null) return;

    //check status
    var isValid = checkStatus();
    if (!isValid) return;

    var $confirmLock = $("#modalConfirmYesNo");

    //getting checked tr
    var tr = $("input[name='radio']:checked").parent().parent();
    var username = tr[0].children[3].innerText;
    $("#hMessage").text("Corporate Approve Rule Confirmation");
    $("#iMessage").text("Are you sure you want to approve ?");
    $confirmLock.modal('show');
    $('#btnYesConfirmYesNo').on('click', function () {
        //alert(radioValue);
        var obj = {
            id: parseInt(id)
        };
        //set signin pwd
        $.ajax({
            type: "POST",
            url: "/C_ApproveRule/Approve",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            data: JSON.stringify(obj),
            cache: false,
            success: function (data) {
                $("#pMessage").text(data.msg);
                $("#divSuccess").modal('toggle');
                if (data.filename != null) {
                    //download pdf
                    download(data.filename);
                }
            },
            error: function (xhr, status, error) {
                if (status != null) {
                    approvePermission();
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

//reject
function reject() {
    var id = GetID();
    if (id == null) return;

    //check status
    var isValid = checkStatus();
    if (!isValid) return;

    //getting checked tr
    var tr = $("input[name='radio']:checked").parent().parent();
    $("#DivModal").modal('toggle');


    $('#btnSave').on('click', function () {
        var remarkvalue = $("#Remark").val().trim();
        var data = { id: id, remark: remarkvalue };

        if (data.remark == "") {
            $("#eMessage").text("*Reason is required!");
            return false;
        }

        $.ajax({
            type: "POST",
            url: "/C_ApproveRule/Reject",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            data: JSON.stringify(data),
            cache: false,
            success: function (data) {
                $("#pMessage").text(data);
                $("#divSuccess").modal('toggle');
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    });
}


//check user data details for Approve
function approveddetails() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/C_ApproveRule/ApprovedDetails/" + id;
}

//permission message for approver
function approvePermission() {
    $("#errorMessage").text("You don't have permission !");
    $("#approvePermissionError").modal('toggle');
}

//to check status "requested"
function checkStatus() {
    //getting checked tr
    var tr = $("input[name='radio']:checked").parent().parent();
    console.log(tr);
    var status = tr[0].lastElementChild.innerText;
    console.log(status);

    if (status != 'Requested') {
        if (status == 'Approved') {
            $("#pMessage").text("Already approved!");
        } else if (status == 'Rejected') {
            $("#pMessage").text("Already rejected!");
        } else {
            $("#pMessage").text("You can't approve/reject. This request isn't at the requested state!");
        }
        $("#divSuccess").modal('toggle');
        return false;
    }

    return true;
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