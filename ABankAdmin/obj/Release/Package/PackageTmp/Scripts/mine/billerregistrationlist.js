//
//const DateFormat = 'MM-DD-YYYY';
initialize();

function initialize() {
    showDataOnTable();
    $("#btnEdit").prop('disabled', true);
    $("#btnDetails").prop('disabled', true);
    $("#btnStatus").prop('disabled', true);
}

//show biller list
function showDataOnTable() {
    var actionurl = '/Biller/GetBillerList';
    /*disable datatable warnings*/
    $.fn.dataTable.ext.errMode = 'none';
    table = $('#myTable').DataTable({
        destroy: true,
        responsive: true,
        processing: true, // for show progress bar
        serverSide: true, // for process server side
        searching: false,
        paging: true,
        ordering: true,
        info: false,
        ajax: {
            url: actionurl,
            type: 'POST',
            data: {
                Name: $("#Name").val(),
                BillerCode: $("#BillerCode").val(),
                BillerType: $("#BillerType").val(),
                Currency: $("#Currency").val(),
                Status: $("#Status").val()
            },
            datatype: 'json',
            async: true,
            error: function (xhr, ajaxError, thrown) {
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
                "render": function (data, type, row, meta) {
                    return `<input type="radio" name="radio" class="checkRadio" data-id='${data}' data-status='${row.Active}' >`;
                }
            },
            {
                targets: [1],
                searchable: false,
                orderable: true,
                class: "wrapok"
            },
            {
                targets: [2, 3, 4, 5],
                searchable: false,
                orderable: true
            },
            {
                targets: [6],
                searchable: false,
                orderable: true,
                "render": function (data) {
                    var status;
                    switch (data) {
                        case false:
                            status = "Inactive";
                            break;
                        case true:
                            status = "Active";
                            break;
                        default:
                            status = "Unknown";
                    }
                    return status;
                }
            }
        ],
        columns: [
            { data: 'ID', name: 'ID', title: '' },
            { data: 'Name', name: 'Name', title: 'Name', autoWidth: true },
            { data: 'BillerCode', name: 'BillerCode', title: 'Biller Code', autoWidth: true },
            { data: 'CreditAccountNo', name: 'CreditAccountNo', title: 'Credit Account No.', autoWidth: true },
            { data: 'BillerType', name: 'BillerType', title: 'Biller Type', autoWidth: true },
            { data: 'Currency', name: 'Currency', title: 'Currency', autoWidth: true },
            { data: 'Active', name: 'Active', title: 'Status', autoWidth: true }
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

        $("#btnEdit").prop('disabled', false);
        $("#btnDetails").prop('disabled', false);
        $("#btnStatus").prop('disabled', false);

        if (status == true) {
            $("#btnStatus").text('Disable');
        } else {
            $("#btnStatus").text('Enable');
        }

        $('#billerId').val(id);
        $('#billerStatus').val(status);
    });

    //edit
    $("#btnEdit").click(function (e) {
        var id = $('#billerId').val();
        if (id) {
            window.location.href = '/Biller/EditBiller/' + id;
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });

    //details
    $("#btnDetails").click(function (e) {
        var id = $('#billerId').val();
        if (id) {
            window.location.href = '/Biller/DetailBiller/' + id;
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });

    //delete
    $("#btnStatus").click(function (e) {
        var id = $('#billerId').val();
        var status = $('#billerStatus').val();
        var messageStatus = "Enable";
        var billerUrl = "/Biller/EnableBiller";
        if (status == "true") {
            messageStatus = "Disable";
            billerUrl = "/Biller/DisableBiller";
        }
        if (id) {
            bootbox.confirm({
                size: "small",
                message: `Are you sure you want to ${messageStatus}?`,
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
                        updateData(id, billerUrl);
                    }
                }
            });
        } else {
            showAlert("Error", "You need to select one of the row in table!");
        }
    });
});

//delete data
function updateData(id, billerUrl) {
    var form = $('#BillerList');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: billerUrl,
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
    $("#BillerList").validate({
        rules: {
            //Name: { required: true }
        },
        messages: {
            //Name: { required: "The FieldName field is required!"
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