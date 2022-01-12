//
initialize();

function initialize() {
    showDataOnTable();
}

//show user registration list
function showDataOnTable() {
    var actionurl = '/CorporateUserRequest/GetCorporateUserList';
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
                PhoneNo: $("#phoneNo").val(),
                UserRole: $("#userRole").val()
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
        order: [[1, 'asc']],
        columnDefs: [
            {
                targets: [0],
                searchable: false,
                orderable: false,
                // Create checkbox
                "render": function (data) {
                    return '<input type="radio" name="radio" class="checkRadio" data-id=' + data + '>';
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
                orderable: false,
                // Create checkbox
                "render": function (data) {
                    var is_checked = data == true ? "checked" : "";
                    return '<input type="checkbox" disabled class="checkbox signincheckbox" ' + is_checked + ' />';
                }
            },
            {
                targets: [8],
                searchable: false,
                orderable: false,
                // Create checkbox
                "render": function (data) {
                    var is_checked = data == true ? "checked" : "";
                    return '<input type="checkbox" disabled class="checkbox transcheckbox" ' + is_checked + ' />';
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
            { data: 'Signinlock', name: 'Signinlock', title: 'SIGNIN_LOCK', autoWidth: true },
            { data: 'Translock', name: 'Translock', title: 'TRANACTION_LOCK', autoWidth: true }
        ]
    });
}

$('document').ready(function () {
    //check radio by row click
    $(document).on('click', '#myTable tbody tr', function () {
        var $this = $(this).children('td:first').children('.checkRadio');
        $this.prop("checked", true);
        var id = $this.data("id");
        $('#regId').val(id);
        if ($(this).children('td:nth-child(8)').children('.signincheckbox').is(":checked")) {
            $('#signinStatus').val(true);
        }
        if ($(this).children('td:nth-child(9)').children('.transcheckbox').is(":checked")) {
            $('#tranStatus').val(true);
        }
    });

    //edit
    $("#btnEdit").click(function (e) {
        var id = $('#regId').val();
        if (id) {
            window.location.href = '/CorporateUserRequest/EditCorporateUser/' + id;
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });

    //details
    $("#btnDetails").click(function (e) {
        var id = $('#regId').val();
        if (id) {
            window.location.href = '/CorporateUserRequest/DetailCorporateUser/' + id;
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

    //reset
    $("#btnReset").click(function (e) {
        var id = $('#regId').val();
        if (id) {
            bootbox.confirm({
                size: "small",
                message: "Are you sure you want to Reset Password?",
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-primary'
                    },
                    cancel: {
                        label: 'No',
                        className: 'btn-default'
                    }
                },
                callback: function (result) {
                    if (result) {
                        resetPassword(id);
                    }
                }
            });
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });

    //signin
    $("#btnSignin").click(function (e) {
        var id = $('#regId').val();
        var status = $('#signinStatus').val();
        if (id) {
            bootbox.confirm({
                size: "small",
                message: "Are you sure you want to Lock/Unlock Singin?",
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-primary'
                    },
                    cancel: {
                        label: 'No',
                        className: 'btn-default'
                    }
                },
                callback: function (result) {
                    if (result) {
                        lockSignin(id, status);
                    }
                }
            });
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });

    //Transactions
    $("#btnTran").click(function (e) {
        var id = $('#regId').val();
        var status = $('#tranStatus').val();
        if (id) {
            bootbox.confirm({
                size: "small",
                message: "Are you sure you want to Lock/Unlock Transaction?",
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-primary'
                    },
                    cancel: {
                        label: 'No',
                        className: 'btn-default'
                    }
                },
                callback: function (result) {
                    if (result) {
                        lockTrans(id, status);
                    }
                }
            });
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });
});

$(document).ready(function () {
    //validate form
    //$("#CorporateUserList").validate({
    //    rules: {
    //        //fromDate: { smallerDate: "#toDate" },
    //        //toDate: { greaterDate: "#fromDate" }
    //    }, messages: {
    //        //fromDate: "Must be less than or equal to To Date!",
    //        //toDate: "Must be greater than or equal to From Date!"
    //    },
    //    errorElement: "span",
    //    errorClass: "help-block",
    //    errorPlacement: function errorPlacement(error, element) {
    //        error.addClass('invalid-feedback text-danger-important');
    //        element.next().is('.input-group-addon') ? error.insertAfter(element.parent()) : error.insertAfter(element);
    //    },
    //    highlight: function highlight(element) { $(element).addClass('is-invalid'); },
    //    unhighlight: function unhighlight(element) { $(element).removeClass('is-invalid'); },
    //    submitHandler: function (form, event) {
    //        event.preventDefault();
    //        showDataOnTable();
    //    }
    //});

    $("#CorporateUserList").submit(function (e) {
        //prevent Default functionality
        e.preventDefault();

        //get the action-url of the form
        //var actionurl = e.currentTarget.action;

        //do your own request an handle the results
        showDataOnTable();
    });
});

//delete data
function deleteData(id) {
    var form = $('#CorporateUserList');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: "/CorporateUserRequest/DeleteCorporateUserReg",
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

//reset password
function resetPassword(id) {
    var form = $('#CorporateUserList');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: "/CorporateUserRequest/ResetPasswordCorporateUserReg",
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

//lock signin
function lockSignin(id, status) {
    var url;
    if (status) {
        url = "/CorporateUserRequest/SigninUnLockCorporateUserReg";
    } else {
        url = "/CorporateUserRequest/SigninLockCorporateUserReg";
    }
    var form = $('#CorporateUserList');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: url,
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

//lock transaction
function lockTrans(id, status) {
    var url;
    if (status) {
        url = "/CorporateUserRequest/TransUnLockCorporateUserReg";
    } else {
        url = "/CorporateUserRequest/TransLockCorporateUserReg";
    }
    var form = $('#CorporateUserList');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: url,
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

//show message
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