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

//edit user by select id
function editreq() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/C_ApproveRule/EditReqApproveRule/" + id;
}

//edit user by select id
function edit() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/C_ApproveRule/Edit/" + id;
}

//Approve Rule Request Detail
function detailreq() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/C_ApproveRule/DetailsReqApproveRule/" + id;
}

//Approve Rule Details
function detail() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/C_ApproveRule/Details/" + id;
}

//delete approve rule by id
function deletereq() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/C_ApproveRule/DeleteReqApproveRule/" + id;

}

//delete approve rule by id
function deletelst() {
    var id = GetID();
    if (id == null) return;
    window.location.href = "/c_approverule/delete/" + id;
}


function AppRuleDeleteConfirmed(id) {
    var obj = {
        id: parseInt(id)
    };
    if (id == null) return;
    $.ajax({
        type: "POST",
        url: "/c_approverule/delete",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        //async: false,
        data: JSON.stringify(obj),
        cache: false,
        success: function (data) {
            $("#pMessage").text(data);
            $("#divARSuccess").modal('toggle');
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