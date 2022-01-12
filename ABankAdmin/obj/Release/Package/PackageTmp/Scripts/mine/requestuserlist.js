//
const DateFormat = 'MM-DD-YYYY';
initialize();

function initialize() {
    showDataOnTable();
}

$(function () {
    //date input initialize
    $('#fromDate').datetimepicker({
        format: DateFormat,
        //maxDate: new Date(),
        //defaultDate: new Date()
        //clearBtn: true
    });

    //date input initialize
    $("#toDate").datetimepicker({
        format: DateFormat,
        //maxDate: new Date(),
        //: new Date()
        //clearBtn: true
    });

    //calander add on click
    $('.input-group-addon').click(function () {
        $(this).closest(".input-group").find('.inputField').focus();
    });
});

//show request users list
function showDataOnTable() {
    var actionurl = '/CorporateUserRequest/GetRequestUserList';
    /*disable datatable warnings*/
    $.fn.dataTable.ext.errMode = 'none';
    table = $('#myTable').DataTable({
        destroy: true,
        responsive: true,
        //processing: true, // for show progress bar
        serverSide: true, // for process server side
        searching: false,
        paging: true,
        ordering: true,
        info: false,
        ajax: {
            url: actionurl,
            type: 'POST',
            data: {
                CompanyName: $("#companyName").val(),
                CorporateID: $("#corporateID").val(),
                UserName: $("#userName").val(),
                UserRole: $("#userRole").val(),
                UserType: $("#userType").val(),
                FromDate: $("#fromDate").val(),
                ToDate: $("#toDate").val(),
                Status: $("#status").val()
            },
            datatype: 'json',
            async: true,
            error: function(xhr, ajaxError, thrown) {
                if (xhr.status == 302) {
                    window.location.href = "/Admin/Login";
                }
            }
            //error: function (xhr, textStatus, exceptionThrown) {
            //    var errorData = $.parseJSON(xhr.responseText);
            //    var errorMessages = [];
            //    for (var key in errorData) {
            //        errorMessages.push(errorData[key]);
            //    }
            //    showAlert(exceptionThrown, errorMessages.join("<br />"), true);
            //}
        },
        order: [[7, 'asc']],
        columnDefs: [
            {
                targets: [0],
                searchable: false,
                orderable: false,
                // Create checkbox
                "render": function (data, type, row, meta) {
                    return '<input type="radio" name="radio" class="checkRadio" data-id=' + data + ' data-isdeleteable=' + row.IsDeleteable + ' data-status=' + row.Status + ' > ';
                }
            },
            {
                targets: [1, 4],
                searchable: false,
                orderable: true,
                class: "wrapok"
            },
            {
                targets: [2, 3, 5],
                searchable: false,
                orderable: true
            },
            {
                targets: [6],
                searchable: false,
                orderable: false,
                class: "wrapok"
            },
            {
                targets: [7],
                searchable: false,
                orderable: true,
                "render": function (data) {
                    var date = new Date(parseInt(data.substr(6)));
                    return date.toLocaleDateString('en-GB').split("/").join("-");
                }
            },
            {
                targets: [8],
                searchable: false,
                orderable: true,
                "render": function (data) {
                    var type;
                    switch (data) {
                        case true:
                            type = "Register";
                            break;
                        default:
                            type = "Update";
                    }
                    return type;
                }
            },
            {
                targets: [9],
                searchable: false,
                orderable: true,
                "render": function (data) {
                    var status;
                    switch (data) {
                        case 0:
                            status = "Requested";
                            break;
                        case 1:
                            status = "Approved";
                            break;
                        case 2:
                            status = "Rejected";
                            break;
                        default:
                            status = "Unknown";
                    }
                    return status;
                }
            }
        ],
        columns: [
            { data: 'Id', name: 'Id', title: '' },
            { data: 'CompanyName', name: 'CompanyName', title: 'COMPANY NAME', autoWidth: true },
            { data: 'CorporateId', name: 'CorporateId', title: 'CORPORATE ID', autoWidth: true },
            { data: 'Username', name: 'Username', title: 'USERNAME', autoWidth: true },
            { data: 'Fullname', name: 'Fullname', title: 'FULLNAME', autoWidth: true },
            { data: 'Phoneno', name: 'Phoneno', title: 'PHONE NO.', autoWidth: true },
            { data: 'Userrole', name: 'Userrole', title: 'USER ROLE', autoWidth: true },
            { data: 'RequestedDate', name: 'RequestedDate', title: 'REQUESTED DATE', autoWidth: true },
            { data: 'Type', name: 'Type', title: 'TYPE', autoWidth: true },
            { data: 'Status', name: 'Status', title: 'STATUS', autoWidth: true }
        ]
    });
}

$('document').ready(function () {
    //check radio by row click
    $(document).on('click', '#myTable tbody tr', function () {
        var $this = $(this).children('td:first').children('.checkRadio');
        $this.prop("checked", true);
        var id = $this.data("id");
        var status = $this.data("status");
        if (status == '1' || status == '2') {
            $("#btnEdit").prop('disabled', true);
        } else {
            $("#btnEdit").prop('disabled', false);
        }

        var isDeleteable = $this.data("isdeleteable");
        if (isDeleteable) {
            $("#btnDelete").prop('disabled', false);
        } else {
            $("#btnDelete").prop('disabled', true);
        }

        $('#regId').val(id);
        $('#regStatus').val(status);
    });

    //edit
    $("#btnEdit").click(function (e) {
        var id = $('#regId').val();
        var status = $('#regStatus').val();
        if (id) {
            window.location.href = '/CorporateUserRequest/EditReqCorporateUser/' + id;
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });

    //details
    $("#btnDetails").click(function (e) {
        var id = $('#regId').val();
        if (id) {
            window.location.href = '/CorporateUserRequest/DetailReqCorporateUser/' + id;
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });

    //delete
    $("#btnDelete").click(function (e) {
        var id = $('#regId').val();
        if (id) {
            bootbox.confirm({
                size: "small",
                message: "Are you sure you want to Delete?",
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-danger'
                    },
                    cancel: {
                        label: 'No',
                        className: 'btn-default'
                    }
                },
                callback: function (result) {
                    if (result) {
                        deleteData(id);
                    }
                }
            });
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });
});

//delete data
function deleteData(id) {
    var form = $('#RequestUserList');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: "/CorporateUserRequest/DeleteRequestUser",
        type: "POST",
        data: {
            __RequestVerificationToken: token,
            id: id
        },
        async: true,
        dataType: "json"
    }).done(function (response) {
        showAlert(response.title, response.message, true);
    }).fail(function (xhr, textStatus, exceptionThrown) {
        var errorData = $.parseJSON(xhr.responseText);
        var errorMessages = [];
        for (var key in errorData) {
            errorMessages.push(errorData[key]);
        }
        showAlert(exceptionThrown, errorMessages.join("<br />"));
    });
}

$(document).ready(function () {
    //validate form
    $("#RequestUserList").validate({
        rules: {
            fromDate: { smallerDate: "#toDate" },
            toDate: { greaterDate: "#fromDate" }
        }, messages: {
            fromDate: "Must be less than or equal to To Date!",
            toDate: "Must be greater than or equal to From Date!"
        },
        errorElement: "span",
        errorClass: "help-block",
        errorPlacement: function errorPlacement(error, element) {
            error.addClass('invalid-feedback text-danger-important');
            element.next().is('.input-group-addon') ? error.insertAfter(element.parent()) : error.insertAfter(element);
        },
        highlight: function highlight(element) { $(element).addClass('is-invalid'); },
        unhighlight: function unhighlight(element) { $(element).removeClass('is-invalid'); },
        submitHandler: function (form, event) {
            event.preventDefault();
            showDataOnTable();
        }
    });
});

//check from date and to date
jQuery.validator.addMethod("greaterDate",
    function (value, element, param) {
        var paramValue = $(param).val();
        var valueDate = moment(value, DateFormat);
        var paramDate = moment(paramValue, DateFormat);
        return this.optional(element) || !paramValue || valueDate.toDate() >= paramDate.toDate();
    }, 'Must be greater than {0}.');

jQuery.validator.addMethod("smallerDate",
    function (value, element, param) {
        var paramValue = $(param).val();
        var valueDate = moment(value, DateFormat);
        var paramDate = moment(paramValue, DateFormat);
        return this.optional(element) || !paramValue || valueDate.toDate() <= paramDate.toDate();
    }, 'Must be smaller than {0}.');

//show alert message
function showAlert(title, message, reload = false) {
    bootbox.alert({
        size: "small",
        title: title,
        message: message,
        callback: function () {
            if (reload) {
                location.reload();
            }
        }
    })
}