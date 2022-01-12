//
const DateFormat = 'MM-DD-YYYY';
const DateDisplayFormat = 'DD/MM/YYYY hh:mm:ss A';
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
    });

    //date input initialize
    $("#toDate").datetimepicker({
        format: DateFormat,
        //maxDate: new Date(),
        //defaultDate: new Date()
    });

    //calander add on click
    $('.input-group-addon').click(function () {
        $(this).closest(".input-group").find('.inputField').focus();
    });
});

//show request changes list
function showDataOnTable() {
    var actionurl = '/CorporateUserRequest/GetRequestChangesList';
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
        order: [[5, 'asc']],
        columnDefs: [
            {
                targets: [0],
                searchable: false,
                orderable: true,
                class: "wrapok"
            },
            {
                targets: [1, 2, 4],
                searchable: false,
                orderable: true
            },
            {
                targets: [3],
                searchable: false,
                orderable: true,
                class: "wrapok",
                "render": function (data) {
                    var type;
                    switch (data) {
                        case 2:
                            type = "Reset Password";
                            break;
                        case 3:
                            type = "Lock";
                            break;
                        case 4:
                            type = "Unlock";
                            break;
                        case 5:
                            type = "Tran Lock";
                            break;
                        case 6:
                            type = "Tran Unlock";
                            break;
                        case 7:
                            type = "Delete";
                            break;
                        default:
                            type = "Unknown";
                    }
                    return type;
                }
            },
            {
                targets: [5],
                searchable: false,
                orderable: true,
                "render": function (data) {
                    var date = new Date(parseInt(data.substr(6)));
                    return date.toLocaleDateString('en-GB').split("/").join("-");
                }
            },
            {
                targets: [6],
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
            },
            {
                targets: [7],
                searchable: false,
                orderable: false,
                "render": function (data, type, row, meta) {
                    var isDisable = '';
                    if (!row.IsDeleteable) {
                        isDisable = 'disabled';
                    }
                    var btnDetails = '<input type="button" value="Details" class="btn btn-default" name="btnDetails" data-id=' + data + '>';
                    var btnDelete = '<input type="button" value="Delete" class="btn btn-danger" name="btnDelete" data-id=' + data + ' ' + isDisable + '>';
                    return btnDelete + ' | ' + btnDetails;
                }
            }
        ],
        columns: [
            { data: 'CompanyName', name: 'CompanyName', title: 'COMPANY NAME', autoWidth: true },
            { data: 'CorporateId', name: 'CorporateId', title: 'CORPORATE ID', autoWidth: true },
            { data: 'Username', name: 'Username', title: 'USERNAME', autoWidth: true },
            { data: 'Type', name: 'Type', title: 'TYPE', autoWidth: true },
            { data: 'Maker', name: 'Maker', title: 'MAKER', autoWidth: true },
            { data: 'RequestedDate', name: 'RequestedDate', title: 'REQUESTED DATE', autoWidth: true },
            { data: 'Status', name: 'Status', title: 'STATUS', autoWidth: true },
            { data: 'Id', name: 'Id', title: 'ACTIONS' }
        ]
    });
}

$('document').ready(function () {
    //detail click
    $(document).on('click', 'input[type=button][name=btnDetails]', function () {
        var id = $(this).data("id");
        if (id) {
            const actionUrl = '/CorporateUserRequest/GetRequestChanges';
            $.post(actionUrl,
                {
                    id: id
                },
                function (response) {
                    var ele = CreateViewData(response);
                    if (ele) {
                        bootbox.dialog({
                            title: 'Request Changes Details',
                            message: ele,
                            size: 'medium',
                            buttons: {
                                ok: {
                                    label: "OK",
                                    className: 'btn-info'
                                }
                            }
                        });
                    }
                }).fail(function (xhr, textStatus, exceptionThrown) {
                    var errorData = $.parseJSON(xhr.responseText);
                    var errorMessages = [];
                    for (var key in errorData) {
                        errorMessages.push(errorData[key]);
                    }
                    showAlert(exceptionThrown, errorMessages.join("<br />"));
                });
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });

    //delete click
    $(document).on('click', 'input[type=button][name=btnDelete]', function () {
        var id = $(this).data("id");
        if (id) {
            const actionUrl = '/CorporateUserRequest/GetRequestChanges';
            $.post(actionUrl,
                {
                    id: id
                },
                function (response) {
                    var ele = CreateViewData(response);
                    if (ele) {
                        bootbox.dialog({
                            title: 'Are you sure you want to Delete?',
                            message: ele,
                            size: 'medium',
                            buttons: {
                                cancel: {
                                    label: "No",
                                    className: 'btn-default'
                                },
                                ok: {
                                    label: "Yes",
                                    className: 'btn-danger',
                                    callback: function () {
                                        deleteData(id);
                                    }
                                }
                            }
                        });
                    }
                }).fail(function (xhr, textStatus, exceptionThrown) {
                    var errorData = $.parseJSON(xhr.responseText);
                    var errorMessages = [];
                    for (var key in errorData) {
                        errorMessages.push(errorData[key]);
                    }
                    showAlert(exceptionThrown, errorMessages.join("<br />"));
                });
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });
});

//delete data
function deleteData(id) {
    var form = $('#RequestChangesList');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: "/CorporateUserRequest/DeleteRequestChanges",
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
    $("#RequestChangesList").validate({
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
    }, 'Must be less than {0}.');

//create view Data for popup form
function CreateViewData(data) {
    var dl = '<dl class="dl-horizontal">{0}</dl>'
    var str = '<dt>{0}</dt><dd>{1}</dd>';
    var checkedDate = '-';
    if (data.CheckedDate) {
        checkedDate = moment(data.CheckedDate).format(DateDisplayFormat);
    }
    var element = '';
    element += String.format(str, 'Company Name', data.CompanyName);
    element += String.format(str, 'Username', data.Username);
    element += String.format(str, 'Branch ID', data.BranchName);
    element += String.format(str, 'Type', data.Type);
    element += String.format(str, 'Status', data.Status);
    element += String.format(str, 'Maker', data.Maker);
    element += String.format(str, 'Requested Date', moment(data.RequestedDate).format(DateDisplayFormat));
    element += String.format(str, 'Checker', data.Checker);
    element += String.format(str, 'Checked Date', checkedDate);
    element += String.format(str, 'Checked Reason', data.CHeckedReason);
    return String.format(dl, element);
}

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

//string format
if (!String.format) {
    String.format = function (format) {
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
                ? args[number]
                : match
                ;
        });
    };
}