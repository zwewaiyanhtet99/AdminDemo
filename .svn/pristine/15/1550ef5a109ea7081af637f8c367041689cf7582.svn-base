const billerListUrl = '/Biller/BillerList';
const getBillerUrl = '/Biller/GetBiller';
const TextFormField = ['FieldType', 'FieldName', 'LableName', 'LableNameMM', 'DefaultValue', 'Placeholder', 'MaxLength', 'MinLength', 'Required', 'IsInput', 'IsOutput', 'IsHidden'];
const NumberFormField = ['FieldType', 'FieldName', 'LableName', 'LableNameMM', 'DefaultValue', 'Placeholder', 'MaxLength', 'MinLength', 'Required', 'IsInput', 'IsOutput', 'IsHidden'];
const DropdownFormField = ['FieldType', 'FieldName', 'LableName', 'LableNameMM', 'Required', 'IsInput', 'IsOutput', 'IsHidden'];
const ListFormField = ['FieldType', 'FieldName', 'LableName', 'LableNameMM', 'Required', 'IsInput', 'IsOutput', 'IsHidden'];
const ListItemFormField = ['FieldType', 'FieldName', 'LableName', 'LableNameMM', 'Required', 'IsInput', 'IsOutput', 'IsHidden'];
const tableTitle = ['Type', 'Name', 'Label', 'Label(MM)', 'Required', 'Input', 'Output', 'Hidden', 'Action'];
const fieldTypeList = 'FieldTypeList';
const defaultField = [{ FieldType: 'Number', FieldName: 'Transactionamount' },
{ FieldType: 'Textbox', FieldName: 'Remark' }];

//initialize
function initialize() {
    //biller id
    var id = $('#BillerId').val();

    //for update, detail or create
    switch ($('#BillerStatus').val()) {
        case '2':
            //get biller data
            GetBillerData(id, true);
            break;
        case '3':
            //get biller data
            GetBillerData(id);
            break;
        default:
            ChangeToAddStatus();
            CreateDefaultField();
    }
}

//get biller data by id
function GetBillerData(id, isEdit = false) {
    var dialog = bootbox.dialog({
        //title: 'Biller Information',
        size: 'small',
        message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> Loading...</div>',
        closeButton: false
    });

    $.post(getBillerUrl, {
        id: id
    },
        function (response) {
            ModelToView(response.biller, response.billerFields, isEdit);
        }).fail(function (xhr, textStatus, exceptionThrown) {
            var errorData = $.parseJSON(xhr.responseText);
            var errorMessages = [];
            for (var key in errorData) {
                errorMessages.push(errorData[key]);
            }
            showAlert(exceptionThrown, errorMessages.join("<br />"), billerListUrl);
            dialog.modal('hide');
        }).done(function () {
            switch (isEdit) {
                case true:
                    //update
                    ChangeToUpdateStatus();
                    break;
                default:
                    //detail
                    ChangeToDetailStatus();
            }
            dialog.modal('hide');
        });
}

//CreateDefaultField
function CreateDefaultField(isApi = false) {
    var jsonItemsData = [];
    var TransactionamountData = {
        "FieldName": "Transactionamount",
        "FieldType": "Number",
        "LableNameMM": "TransactionAmount",
        "LableName": "TransactionAmount",
        "DefaultValue": "0",
        "Placeholder": "",
        "IsOutput": false,
        "IsInput": true,
        "IsHidden": false,
        "Attributes": {
            "MaxLength": 20,
            "MinLength": 1,
            "Required": true
        }
    };
    var RemarkData = {
        "FieldName": "Remark",
        "FieldType": "Textbox",
        "LableNameMM": "Description",
        "LableName": "Description",
        "DefaultValue": "",
        "Placeholder": "",
        "IsOutput": false,
        "IsInput": true,
        "IsHidden": false,
        "Attributes": {
            "Required": false
        }
    };

    if (!isApi) {
        jsonItemsData.push(TransactionamountData);
    }
    jsonItemsData.push(RemarkData);

    var $droppableUL = $('ul#droppable li');
    if ($droppableUL.length != 0) {
        var biller_Fields = GetBillerFields();
        $.each(biller_Fields, (key, value) => {
            if (checkDefaultField(value.FieldName, value.FieldType) == false) {
                jsonItemsData.push(value);
            }
        });
        $droppableUL.remove();
        //biller_Fields = biller_Fields.filter(function (obj) {
        //    return checkDefaultField(obj.FieldName, obj.FieldType) == false;
        //});
    }

    $.each(jsonItemsData, function (index, val) {
        var $item = createLiElement(val);
        $('#droppable').append($item);
    });
}

$(document).ready(function () {
    //number
    $('.number-separator').number(true, 2);

    //Initialize tooltips
    $('.sort-item > span[title]').tooltip();
    $('.remove-item > span[title]').tooltip();
    $('span.edit-table-row[title]').tooltip();
    $('span.delete-table-row[title]').tooltip();
    $('.list-group > li[title]').tooltip();
    //$('.nav-tabs > li a[title]').tooltip();

    //dynamic form cancel
    $("button[name='dynamic_form_cancel']").on('click', function (e) {
        e.preventDefault();
        dynamic_form.resetForm();
        $('#dynamic_form').trigger("reset");
        //$('#droppable .ui-selected').removeClass('ui-selected')
        // $('#droppable').trigger('unselected');
        $('a[href="#AddField"]').click();
        disableEditableForm();
    });

    //droppable
    $("#droppable")
        .sortable({
            handle: ".sort-field",
            axis: "y",
            cursor: "move",
            receive: function (event, ui) {
                var item = $(ui.item).attr("data-value");
                switch (item) {
                    case 'Textbox':
                        var text_data = {
                            "FieldName": "New",
                            "FieldType": "Textbox",
                            "LableNameMM": "",
                            "LableName": "New",
                            "DefaultValue": "",
                            "Placeholder": "",
                            "IsOutput": false,
                            "IsInput": true,
                            "IsHidden": false,
                            "SortOrder": "",
                            "Attributes": {
                                //"MaxLength": 50,
                                //"MinLength": 5,
                                "Required": true
                            }
                        };
                        $(this).find('li.ui-draggable-dragging')
                            .replaceWith(createLiElement(text_data));
                        break;
                    case 'Number':
                        var number_data = {
                            "FieldName": "New",
                            "FieldType": "Number",
                            "LableNameMM": "",
                            "LableName": "New",
                            "DefaultValue": "",
                            "Placeholder": "",
                            "IsOutput": false,
                            "IsInput": true,
                            "IsHidden": false,
                            "SortOrder": "",
                            "Attributes": {
                                //"MaxLength": 50,
                                //"MinLength": 5,
                                "Required": true
                            }
                        };
                        $(this).find('li.ui-draggable-dragging')
                            .replaceWith(createLiElement(number_data));
                        break;
                    case 'Selectbox':
                        var dropdown_data = {
                            "FieldName": "New",
                            "FieldType": "Selectbox",
                            "LableNameMM": "",
                            "LableName": "New",
                            "IsOutput": true,
                            "IsInput": false,
                            "IsHidden": false,
                            "SortOrder": "",
                            "Attributes": {
                                "Required": true
                            }
                        };
                        $(this).find('li.ui-draggable-dragging')
                            .replaceWith(createLiElement(dropdown_data));
                        break;
                    case 'List':
                        var list_data = {
                            "FieldName": "New",
                            "FieldType": "List",
                            "LableNameMM": "",
                            "LableName": "New",
                            "DefaultValue": "",
                            "Placeholder": "",
                            "IsOutput": true,
                            "IsInput": false,
                            "IsHidden": false,
                            "SortOrder": "",
                            "Attributes": {
                                "Required": true
                            },
                            "Children": []
                        };
                        $(this).find('li.ui-draggable-dragging')
                            .replaceWith(createLiElement(list_data));
                        break;
                }
            },
            start: function (event, ui) {
                $(ui.item).css("opacity", "0.6");
            },
            stop: function (event, ui) {
                $(ui.item).css("opacity", "1.0");
            },
            update: function (event, ui) {
                // alert('changed');
            }
        }).selectable({
            filter: "li",
            cancel: ".sort-field,.remove-field,.add-table-row,.edit-table-row,.delete-table-row",
            selected: function (event, ui) {
                $(ui.selected).addClass("ui-selected").siblings().removeClass("ui-selected");

                //get selected input
                var $li = $('#droppable').children('li.ui-selected').first();
                // var label = $li.find('label.label-name').text();
                var id = $li.attr("id");
                var inputType = $li.attr('data-type');
                var isDefault = $li.attr('data-isdefault');
                var $this = $('#EditField');
                $this.attr("data-id", id);
                $this.attr("data-type", inputType);

                var $dynamic_form = $('#dynamic_form');

                //reset form
                $('form.editable-form div.form-group').hide();
                dynamic_form.resetForm();
                $dynamic_form.trigger("reset");

                //remove all data
                $.each($dynamic_form.data(), function (i) {
                    $dynamic_form.removeAttr(`data-${i}`);
                });

                //add field data
                var $input_dynamic_form, $input_dynamic_form_field;
                //check type
                switch (inputType) {
                    case 'Textbox':
                        $input_dynamic_form = $li.find('input[type="text"]').first();
                        $input_dynamic_form_field = TextFormField;
                        break;
                    case 'Number':
                        $input_dynamic_form = $li.find('input[type="number"]').first();
                        $input_dynamic_form_field = NumberFormField;
                        break;
                    case 'Selectbox':
                        $input_dynamic_form = $li.find('select').first();
                        $input_dynamic_form_field = DropdownFormField;
                        break;
                    case 'List':
                        $input_dynamic_form = $li.find('table > thead > tr > input[type="hidden"]').first();
                        $input_dynamic_form_field = ListFormField;
                        break;
                }

                //check detail or default
                if ($('#BillerStatus').val() == '3') {
                    //show relevent form input and load data
                    $.each($input_dynamic_form_field, function (index, val) {
                        $(`.${val}`).show();
                        $dynamic_form.attr(`data-${val}`, true);
                        if (val == 'FieldType') {
                            $(`#dynamic_form select[name=${val}]`).val($input_dynamic_form.attr(`data-${val}`)).prop('disabled', true);
                        } else if (val == 'Required' || val == 'IsInput' || val == 'IsOutput' || val == 'IsHidden') {
                            if ($input_dynamic_form.attr(`data-${val}`) == 'true') {
                                $(`#dynamic_form input[name=${val}]`).prop("checked", true).prop('disabled', true);
                            } else {
                                $(`#dynamic_form input[name=${val}]`).prop("checked", false).prop('disabled', true);
                            }
                        } else {
                            $(`#dynamic_form input[name=${val}]`).val($input_dynamic_form.attr(`data-${val}`)).prop('disabled', true);
                        }
                    });
                    //dynamic_form_submit
                    $('#dynamic_form button[name="dynamic_form_submit"]').text("Close");
                } else {
                    //show relevent form input and load data
                    $.each($input_dynamic_form_field, function (index, val) {
                        $(`.${val}`).show();
                        $dynamic_form.attr(`data-${val}`, true);
                        if (val == 'FieldName') {
                            if (isDefault == 'true') {
                                $(`#dynamic_form input[name=${val}]`).val($input_dynamic_form.attr(`data-${val}`)).prop('disabled', true);
                            } else {
                                $(`#dynamic_form input[name=${val}]`).val($input_dynamic_form.attr(`data-${val}`)).prop('disabled', false);
                            }
                        }
                        else if (val == 'FieldType') {
                            $(`#dynamic_form select[name=${val}]`).val($input_dynamic_form.attr(`data-${val}`)).prop('disabled', true);
                        } else if (val == 'Required' || val == 'IsInput' || val == 'IsOutput' || val == 'IsHidden') {
                            if ($input_dynamic_form.attr(`data-${val}`) == 'true') {
                                $(`#dynamic_form input[name=${val}]`).prop("checked", true).prop('disabled', false);
                            } else {
                                $(`#dynamic_form input[name=${val}]`).prop("checked", false).prop('disabled', false);
                            }
                        } else {
                            $(`#dynamic_form input[name=${val}]`).val($input_dynamic_form.attr(`data-${val}`)).prop('disabled', false);
                        }
                    });
                    //dynamic_form_submit
                    $('#dynamic_form button[name="dynamic_form_submit"]').text("Save");
                }

                //show form
                $dynamic_form.css("display", "block");

                //activate edit view
                var $button = $('a[href="#EditField"]');
                $button.closest('li').removeClass('disabled');
                $button.click();
            },
            unselected: function (event, ui) {
                $('#dynamic_form').css("display", "none");
                if ($('#droppable').children('li.ui-selecting').length == 0) {
                    disableEditableForm();
                    $('a[href="#AddField"]').click();
                }
            }
        });

    //draggable
    $("#draggable li").draggable({
        connectToSortable: "#droppable",
        helper: "clone",
        revert: "invalid"
    }).disableSelection();

    //test
    //AutofillforTesting();

    initialize();

    //disable form
    disableEditableForm();

    // $('#lablename').on('change keyup paste', function (e) {
    //     var id = $('#EditField').attr("data-id");
    //     $(`#${id}`).find('label.label-name').text($(this).val());
    // });

    //remove form input field
    $(document).on("click", ".remove-field", function (e) {
        var $li = $(this).parents('li:first');
        if ($('#BillerStatus').val() == '3') {
            e.preventDefault();
        }
        else if ($li.attr('data-isdefault') == 'true') {
            showAlert("Warning", "You can't remove this item!");
        } else {
            bootbox.confirm({
                size: "small",
                message: "Are you sure you want to remove this item?",
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
                        //$(this).parent().remove();
                        $li.remove();
                        if ($('#droppable').children('li.ui-selected').length == 0) {
                            disableEditableForm();
                            $('a[href="#AddField"]').click();
                        }
                    }
                }
            });
        }
    });

    //add table row
    $(document).on("click", ".add-table-row", function (e) {
        //check detail
        switch ($('#BillerStatus').val()) {
            case '3':
                e.preventDefault();
                break;
            default:
                disableEditableForm();

                var $li = $(this).parents('li:first');
                if (!$li.hasClass("ui-selected")) {
                    $li.addClass("ui-selected");
                }
                var id = $li.attr('id');
                var inputType = $li.attr('data-type');

                var $this = $('#EditField');
                $this.attr("data-id", id);
                $this.attr("data-type", 'ListItem');
                $this.attr("data-newrow", true);

                var $dynamic_form = $('#dynamic_form');

                //reset form
                dynamic_form.resetForm();
                $dynamic_form.trigger("reset");

                //remove all data
                $.each($dynamic_form.data(), function (i) {
                    $dynamic_form.removeAttr(`data-${i}`);
                });

                //show relevent form input and load data
                $.each(ListItemFormField, function (index, val) {
                    if (val == 'FieldType') {
                        $(`.${fieldTypeList}`).show();
                        $dynamic_form.attr(`data-${fieldTypeList}`, true);
                        $(`#dynamic_form select[name=${fieldTypeList}]`).prop('disabled', false);
                    } else {
                        $(`.${val}`).show();
                        $dynamic_form.attr(`data-${val}`, true);
                        $(`#dynamic_form input[name=${val}]`).prop('disabled', false);
                    }
                });

                //dynamic_form_submit
                $('#dynamic_form button[name="dynamic_form_submit"]').text("Save");
                $dynamic_form.css("display", "block");

                var $button = $('a[href="#EditField"]');
                $button.closest('li').removeClass('disabled');
                $button.click();
        }
    });

    //edit table row
    $(document).on("click", ".edit-table-row", function (e) {
        //check detail
        switch ($('#BillerStatus').val()) {
            case '3':
                e.preventDefault();
                break;
            default:
                disableEditableForm();

                var $curRow = $(this).parents('tr:first');
                var curId = $curRow.attr('id');
                var $li = $curRow.parents('li:first');
                if (!$li.hasClass("ui-selected")) {
                    $li.addClass("ui-selected");
                }
                var id = $li.attr('id');
                var inputType = $li.attr('data-type');

                var $this = $('#EditField');
                $this.attr("data-id", id);
                $this.attr("data-type", 'ListItem');
                $this.attr("data-newrow", false);
                $this.attr("data-rowid", curId);

                var $dynamic_form = $('#dynamic_form');

                //reset form
                dynamic_form.resetForm();
                $dynamic_form.trigger("reset");

                //remove all data
                $.each($dynamic_form.data(), function (i) {
                    $dynamic_form.removeAttr(`data-${i}`);
                });

                //show relevent form input and load data
                var $input_dynamic_form = $curRow.children('input[type="hidden"]').first();
                $.each(ListItemFormField, function (index, val) {
                    if (val == 'FieldType') {
                        $(`.${fieldTypeList}`).show();
                        $dynamic_form.attr(`data-${fieldTypeList}`, true);
                        $(`#dynamic_form select[name=${fieldTypeList}]`).val($input_dynamic_form.attr(`data-${val}`)).prop('disabled', false);
                    } else if (val == 'Required' || val == 'IsInput' || val == 'IsOutput' || val == 'IsHidden') {
                        $(`.${val}`).show();
                        $dynamic_form.attr(`data-${val}`, true);
                        if ($input_dynamic_form.attr(`data-${val}`) == 'true') {
                            $(`#dynamic_form input[name=${val}]`).prop("checked", true).prop('disabled', false);
                        } else {
                            $(`#dynamic_form input[name=${val}]`).prop("checked", false).prop('disabled', false);
                        }
                    } else {
                        $(`.${val}`).show();
                        $dynamic_form.attr(`data-${val}`, true);
                        $(`#dynamic_form input[name=${val}]`).val($input_dynamic_form.attr(`data-${val}`)).prop('disabled', false);
                    }
                });

                //dynamic_form_submit
                $('#dynamic_form button[name="dynamic_form_submit"]').text("Save");
                $dynamic_form.css("display", "block");

                var $button = $('a[href="#EditField"]');
                $button.closest('li').removeClass('disabled');
                $button.click();
        }
    });

    //delete table row
    $(document).on("click", ".delete-table-row", function (e) {
        //check detail
        switch ($('#BillerStatus').val()) {
            case '3':
                e.preventDefault();
                break;
            default:
                var $curRow = $(this).parents('tr:first');
                bootbox.confirm({
                    size: "small",
                    message: "Are you sure you want to remove this item?",
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
                            var curId = $curRow.attr('id');
                            var $li = $curRow.parents('li:first');
                            if (!$li.hasClass("ui-selected")) {
                                $li.addClass("ui-selected");
                            }
                            var id = $li.attr('id');
                            var inputType = $li.attr('data-type');

                            var $this = $('#EditField');
                            var $id = $this.attr("data-id");
                            var $inputType = $this.attr("data-type");
                            var $newrow = $this.attr("data-newrow");
                            var $curId = $this.attr("data-rowid");

                            $curRow.remove();
                            if ($inputType == 'ListItem' && $newrow == 'false' && id == $id && curId == $curId) {
                                disableEditableForm();
                                $('a[href="#AddField"]').click();
                            }
                        }
                    }
                });
        }
    });

    // $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    //     var $this=$(e.target);
    //     debugger
    //     var target = $this.attr("href") // activated tab
    //     var id=$this.attr("data-id");
    //     var inputType=$this.attr("data-type");
    //     // alert(target);
    // });

    //check tab is clickable
    $('.panel a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        var $target = $(e.target);

        if ($target.parent().hasClass('disabled')) {
            return false;
        }
    });

    //Wizard
    $('.wizard-inner a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        var $target = $(e.target);
        var $nextElement = $target.parent();
        while ($nextElement.next().is('li')) {
            if (!$nextElement.next().hasClass('disabled')) {
                $nextElement.next().addClass('disabled');
            }
            $nextElement = $nextElement.next();
        }
        if ($target.parent().hasClass('disabled')) {
            return false;
        }
    });

    //next button click
    $(".next-step").click(function (e) {
        var status = $('#BillerStatus').val();
        var $this = $(this);
        var nextVal = $this.val();
        //check status
        switch (status) {
            case '3':
                switch (nextVal) {
                    case 'BillerInfo':
                        var $active_Detail = $('.wizard .wizard-inner .nav-tabs li.active');
                        $active_Detail.next().removeClass('disabled');
                        nextTab($active_Detail);
                        break;
                    case 'Detail':
                        window.location.href = billerListUrl;
                        break;
                }
                break;
            default:
                switch (nextVal) {
                    case 'BillerInfo':
                        if ($("#biller_form").valid()) {
                            var $active = $('.wizard .wizard-inner .nav-tabs li.active');
                            $active.next().removeClass('disabled');
                            nextTab($active);
                        }
                        break;
                    case 'Register':
                        var formLength = $('ul#droppable li').length;
                        if (!$("#biller_form").valid()) {
                            showAlert('Error', 'Your form has some error! Please check again.');
                        } else if (formLength == 0) {
                            showAlert('Error', 'Your form need to have at least 1 item.');
                        } else if (formLength > 6) {
                            showAlert('Error', 'Your form need to have at most 6 items.');
                        } else if (checkDuplicateFieldName() == true) {
                            showAlert('Error', 'Your form has duplicate Field Name! Please check again.');
                        } else {
                            $this.button('loading');

                            var model = ViewToModel();
                            var biller_Fields = GetBillerFields();
                            SaveToServer(model, biller_Fields, $this, resetLoading);
                        }
                        break;
                    case 'Update':
                        var formLengthUpdate = $('ul#droppable li').length;
                        if (!$("#biller_form").valid()) {
                            showAlert('Error', 'Your form has some error! Please check again.');
                        } else if (formLengthUpdate == 0) {
                            showAlert('Error', 'Your form need to have at least 1 item.');
                        } else if (formLengthUpdate > 6) {
                            showAlert('Error', 'Your form need to have at most 6 items.');
                        } else if (checkDuplicateFieldName() == true) {
                            showAlert('Error', 'Your form has duplicate Field Name! Please check again.');
                        } else {
                            $this.button('loading');

                            var id = $('#BillerId').val();
                            var modelUpdate = ViewToModel();
                            var biller_FieldsUpdate = GetBillerFields();
                            UpdateToServer(id, modelUpdate, biller_FieldsUpdate, $this, resetLoading);
                        }
                        break;
                }
        }
    });

    //previous button click
    $(".prev-step").click(function (e) {
        var $active = $('.wizard .wizard-inner .nav-tabs li.active');
        prevTab($active);
    });

    //back to list button click
    $(".back-step").click(function (e) {
        window.location.href = billerListUrl;
    });

    //payment type change
    $('input[type=radio][name=IsFix]').change(function () {
        switch (this.value) {
            case 'FixAmount':
                $(".Fix-Amount").show();
                $(".Charges-Code").hide();
                $(".Discount").hide();
                switch ($('input[type=checkbox][name=DiffChargesAccount]').is(":checked")) {
                    case true:
                        $('.DiffChargesAcc').show();
                        break;
                    default:
                        $('.DiffChargesAcc').hide();
                }
                break;
            case 'ChargesCode':
                $(".Fix-Amount").hide();
                $(".Charges-Code").show();
                $(".Discount").hide();
                $('.DiffChargesAcc').hide();
                break;
            case 'Discount':
                $(".Fix-Amount").hide();
                $(".Charges-Code").hide();
                $(".Discount").show();
                $('.DiffChargesAcc').hide();
                break;
        }
    });

    //DiffChargesAccount
    $("input[type=checkbox][name=DiffChargesAccount]").change(function () {
        switch (this.checked) {
            case true:
                $('.DiffChargesAcc').show();
                break;
            default:
                $('.DiffChargesAcc').hide();
        }
    });

    //IsApiIntegrate
    $("input[type=checkbox][name=IsApiIntegrate]").change(function () {
        CreateDefaultField(this.checked);
    });

    //$('input[type=radio][name=IsFixed]').change(function () {
    //    if (this.value == 'Percentage') {
    //        $("#Discount").removeClass("number-separator");
    //    }
    //    else if (this.value == 'FixedAmount') {
    //        $("#Discount").addClass("number-separator");
    //    }
    //});
});

//validate registration form
$("#biller_form").validate({
    onkeyup: false, //turn off auto validate whilst typing
    rules: {
        Name: {
            required: true,
            minlength: 3,
            maxlength: 200,
            remote: {
                param: {
                    url: "/Biller/CheckBillerName",
                    type: "post",
                    data: {
                        checkData: function () {
                            return $("#Name").val();
                        },
                        checkDataID: function () {
                            return $("#BillerId").val();
                        }
                    },
                    error:
                        function (xhr, ajaxError, thrown) {
                            if (xhr.status == 302) {
                                window.location.href = "/Admin/Login";
                            }
                        }
                },
                depends: function () {
                    return $("#Name").val() != '';
                }
            }
        },
        Image: {
            required: function (element) {
                //return $('#BillerStatus').val() != '2';
                return $(element).data("required") == true;
            }
        },
        BillerCode: {
            required: true,
            minlength: 3,
            maxlength: 200,
            remote: {
                param: {
                    url: "/Biller/CheckBillerCode",
                    type: "post",
                    data: {
                        checkData: function () {
                            return $("#BillerCode").val();
                        },
                        checkDataID: function () {
                            return $("#BillerId").val();
                        }
                    },
                    error:
                        function (xhr, ajaxError, thrown) {
                            if (xhr.status == 302) {
                                window.location.href = "/Admin/Login";
                            }
                        }
                },
                depends: function () {
                    return $("#BillerCode").val() != '';
                }
            }
        },
        FixAmount: {
            required: function (element) {
                return $('input[name="IsFix"]:checked').val() == 'FixAmount';
            },
            min: 0,
            max: 999999999999999999
        },
        ChargesAccountNo: {
            required: function (element) {
                return $('input[name="IsFix"]:checked').val() == 'FixAmount' && $('input[type=checkbox][name=DiffChargesAccount]').is(":checked") == true;
            },
            minlength: 3,
            maxlength: 20,
            NotEqualTo: "#CreditAccountNo",
            remote: {
                param: {
                    url: "/Biller/CheckValidAccNo",
                    type: "post",
                    data: {
                        checkData: function () {
                            return $("#ChargesAccountNo").val();
                        }
                    },
                    dataFilter: function (data) {
                        if (data) {
                            var json = $.parseJSON(data);
                            if (json) {
                                return JSON.stringify(json.result) /* will be "true" or whatever the error message is */
                            }
                        }
                    },
                    complete: function (data) {
                        var isValid = false;
                        /* Additional code to run if the element passes validation */
                        if (data) {
                            var json = $.parseJSON(data.responseText);
                            if (json) {
                                if (json.result === true) {
                                    $('#ChargesAccountName').text(`Account Name: ${json.AccountName}`);
                                    $('#ChargesAccountName').show();
                                    isValid = true;
                                }
                            }
                        }
                        if (!isValid) {
                            $('#ChargesAccountName').hide();
                        }
                    },
                    error: function (xhr, ajaxError, thrown) {
                        if (xhr.status == 302) {
                            window.location.href = "/Admin/Login";
                        }
                    }
                },
                depends: function () {
                    return $("#ChargesAccountNo").val() != '';
                }
            }
        },
        ChargesCode: {
            required: function (element) {
                return $('input[name="IsFix"]:checked').val() == 'ChargesCode';
            }
        },
        Discount: {
            required: function (element) {
                return $('input[name="IsFix"]:checked').val() == 'Discount' && ($('input[name="IsFixed"]:checked').val() == 'Percentage' || $('input[name="IsFixed"]:checked').val() == 'FixedAmount');
            },
            min: 0,
            max: 999999999999999999
        },
        CreditAccountNo: {
            required: true,
            minlength: 3,
            maxlength: 20,
            NotEqualTo: "#ChargesAccountNo",
            remote: {
                param: {
                    url: "/Biller/CheckValidAccNo",
                    type: "post",
                    data: {
                        checkData: function () {
                            return $("#CreditAccountNo").val();
                        }
                    },
                    dataFilter: function (data) {
                        if (data) {
                            var json = $.parseJSON(data);
                            if (json) {
                                return JSON.stringify(json.result) /* will be "true" or whatever the error message is */
                            }
                        }
                    },
                    complete: function (data) {
                        var isValid = false;
                        /* Additional code to run if the element passes validation */
                        if (data) {
                            var json = $.parseJSON(data.responseText);
                            if (json) {
                                if (json.result === true) {
                                    $('#CreditAccountName').text(`Account Name: ${json.AccountName}`);
                                    $('#CreditAccountName').show();
                                    isValid = true;
                                }
                            }
                        }
                        if (!isValid) {
                            $('#CreditAccountName').hide();
                        }
                    },
                    error: function (xhr, ajaxError, thrown) {
                        if (xhr.status == 302) {
                            window.location.href = "/Admin/Login";
                        }
                    }
                },
                depends: function () {
                    return $("#CreditAccountNo").val() != '';
                }
            }
        },
        BillerType: {
            required: true
        },
        Currency: {
            required: true
        }
    },
    messages: {
        Name: {
            required: "The Name field is required!",
            minlength: jQuery.validator.format(
                "The field Name must be a string with a minimum length of {0}."),
            maxlength: jQuery.validator.format(
                "The field Name must be a string with a maximum length of {0}."),
            remote: jQuery.validator.format("{0} is already in use!")
        },
        Image: {
            required: "The Image field is required!",
        },
        BillerCode: {
            required: "The Biller Code field is required!",
            minlength: jQuery.validator.format(
                "The field Biller Code must be a string with a minimum length of {0}."),
            maxlength: jQuery.validator.format(
                "The field Biller Code must be a string with a maximum length of {0}."),
            remote: jQuery.validator.format("{0} is already in use!")
        },
        FixAmount: {
            required: "The Fix Amount field is required!",
            min: jQuery.validator.format("Enter a value greater than or equal to {0}."),
            max: jQuery.validator.format("Enter a value less than {0}.")
        },
        ChargesAccountNo: {
            required: "The Charges Account No. field is required!",
            minlength: jQuery.validator.format(
                "The field Charges Account No. must be a string with a minimum length of {0}."),
            maxlength: jQuery.validator.format(
                "The field Charges Account No. must be a string with a maximum length of {0}.")
        },
        ChargesCode: {
            required: "The Charges Code field is required!"
        },
        Discount: {
            required: "The Discount field is required!",
            min: jQuery.validator.format("Enter a value greater than or equal to {0}."),
            max: jQuery.validator.format("Enter a value less than {0}.")
        },
        CreditAccountNo: {
            required: "The Credit Account No. field is required!",
            minlength: jQuery.validator.format(
                "The field Credit Account No. must be a string with a minimum length of {0}."),
            maxlength: jQuery.validator.format(
                "The field Credit Account No. must be a string with a maximum length of {0}.")
        },
        BillerType: {
            required: "The Biller Type field is required!"
        },
        Currency: {
            required: "The Currency field is required!"
        }
    },
    errorElement: "span",
    errorClass: "help-block",
    errorPlacement: function (b, c) {
        b.addClass("invalid-feedback text-danger-important"),
            "checkbox" === c.prop("type") ? b.insertAfter(c.parent("label")) :
                "radio" === c.prop("type") ? b.insertAfter(c.parent("label").parent(
                    "label")) :
                    b.insertAfter(c)
    },
    //highlight: function (element) {
    //    $(element).closest('.form-group').addClass('has-error');
    //},
    //unhighlight: function (element) {
    //    $(element).closest('.form-group').removeClass('has-error');
    //},
    highlight: function (element) {
        $(element).addClass('is-invalid');
    },
    unhighlight: function (element) {
        $(element).removeClass('is-invalid');
    },
    submitHandler: function (form, event) {
        event.preventDefault();
    }
});

//validate dynamic form
var dynamic_form = $("#dynamic_form").validate({
    //onkeyup: false, //turn off auto validate whilst typing
    rules: {
        FieldType: {
            required: function () {
                return $("#dynamic_form").attr('data-FieldType') == 'true';
            }
        },
        FieldTypeList: {
            required: function () {
                return $("#dynamic_form").attr('data-FieldTypeList') == 'true';
            }
        },
        FieldName: {
            required: function () {
                return $("#dynamic_form").attr('data-FieldName') == 'true';
            },
            maxlength: 200,
            CheckFieldName: true
        },
        LableName: {
            required: function () {
                return $("#dynamic_form").attr('data-LableName') == 'true';
            },
            maxlength: 200
        },
        LableNameMM: {
            required: function () {
                return $("#dynamic_form").attr('data-LableNameMM') == 'true';
            },
            maxlength: 200
        },
        DefaultValue: {
            maxlength: 100
        },
        Placeholder: {
            maxlength: 200
        },
        IsInput: {
            required: function () {
                return $("#dynamic_form").attr('data-IsInput') == 'true' && !$('#dynamic_form input[name=IsOutput]').prop("checked");
            }
        },
        IsOutput: {
            required: function () {
                return $("#dynamic_form").attr('data-IsOutput') == 'true' && !$('#dynamic_form input[name=IsInput]').prop("checked");
            }
        },
        MaxLength: {
            min: 0,
            max: 999999999999999999
        },
        MinLength: {
            min: 0,
            max: 999999999999999999
        }
    },
    messages: {
        FieldType: {
            required: "The Field Type field is required!"
        },
        FieldTypeList: {
            required: "The Field Type field is required!"
        },
        FieldName: {
            required: "The Field Name field is required!",
            maxlength: jQuery.validator.format(
                "The field Field Name must be a value with a maximum length of {0}.")
        },
        LableName: {
            required: "The Lable field is required!",
            maxlength: jQuery.validator.format(
                "The field Lable must be a value with a maximum length of {0}.")
        },
        LableNameMM: {
            required: "The Lable (MM) field is required!",
            maxlength: jQuery.validator.format(
                "The field Lable(MM) must be a value with a maximum length of {0}.")
        },
        DefaultValue: {
            maxlength: jQuery.validator.format(
                "The field Default Value must be a value with a maximum length of {0}.")
        },
        Placeholder: {
            maxlength: jQuery.validator.format(
                "The field Placeholder must be a value with a maximum length of {0}.")
        },
        IsInput: {
            required: "The Field Input/Output field is required!"
        },
        IsOutput: {
            required: "The Field Input/Output field is required!"
        },
        MaxLength: {
            min: jQuery.validator.format("Enter a value greater than or equal to {0}."),
            max: jQuery.validator.format("Enter a value less than {0}.")
        },
        MinLength: {
            min: jQuery.validator.format("Enter a value greater than or equal to {0}."),
            max: jQuery.validator.format("Enter a value less than {0}.")
        }
    },
    errorElement: "span",
    errorClass: "help-block",
    errorPlacement: function (b, c) {
        b.addClass("invalid-feedback text-danger-important"),
            "checkbox" === c.prop("type") ? b.insertAfter(c.parent("label")) :
                "radio" === c.prop("type") ? b.insertAfter(c.parent("label").parent(
                    "label")) :
                    b.insertAfter(c)
    },
    //highlight: function (element) {
    //    $(element).closest('.form-group').addClass('has-error');
    //},
    //unhighlight: function (element) {
    //    $(element).closest('.form-group').removeClass('has-error');
    //},
    highlight: function (element) {
        $(element).addClass('is-invalid');
    },
    unhighlight: function (element) {
        $(element).removeClass('is-invalid');
    },
    submitHandler: function (form, event) {
        //check detail
        switch ($('#BillerStatus').val()) {
            case '3':
                event.preventDefault();
                break;
            default:
                event.preventDefault();
                // $this = $(this);

                var $edit = $('#EditField');
                var id = $edit.attr("data-id");
                var inputType = $edit.attr("data-type");
                var $element = $(`#${id}`);

                var $input_dynamic_form, $input_dynamic_form_field, $newrow, $rowid;
                //check type
                switch (inputType) {
                    case 'Textbox':
                        $input_dynamic_form = $element.find('input[type="text"]').first();
                        $input_dynamic_form_field = TextFormField;
                        break;
                    case 'Number':
                        $input_dynamic_form = $element.find('input[type="number"]').first();
                        $input_dynamic_form_field = NumberFormField;
                        break;
                    case 'Selectbox':
                        $input_dynamic_form = $element.find('select').first();
                        $input_dynamic_form_field = DropdownFormField;
                        break;
                    case 'List':
                        $input_dynamic_form = $element.find('table > thead > tr > input[type="hidden"]').first();
                        $input_dynamic_form_field = ListFormField;
                        break;
                    case 'ListItem':
                        $newrow = $edit.attr("data-newrow");
                        $rowid = $edit.attr("data-rowid");
                        $input_dynamic_form_field = ListItemFormField;
                        break;
                }

                switch (inputType) {
                    case 'ListItem':
                        switch ($newrow) {
                            case 'true':
                                var $listitem_data = [];
                                $.each($input_dynamic_form_field, function (index, val) {
                                    if (val == 'Required' || val == 'MaxLength' || val == 'MinLength') {
                                        var $attributes = $listitem_data["Attributes"];
                                        if (!$attributes) {
                                            $attributes = [];
                                        }
                                        if (val == 'Required') {
                                            $attributes[`${val}`] = $(`#dynamic_form input[name=${val}]`).prop("checked");
                                        } else {
                                            $attributes[`${val}`] = $.trim($(`#dynamic_form input[name=${val}]`).val());
                                        }
                                        $listitem_data["Attributes"] = $attributes;
                                    } else if (val == 'IsInput' || val == 'IsOutput' || val == 'IsHidden') {
                                        $listitem_data[`${val}`] = $(`#dynamic_form input[name=${val}]`).prop("checked");
                                    } else if (val == 'FieldType') {
                                        $listitem_data[`${val}`] = $(`#dynamic_form select[name=${fieldTypeList}]`).val();
                                    } else {
                                        $listitem_data[`${val}`] = $.trim($(`#dynamic_form input[name=${val}]`).val());
                                    }
                                });

                                var $tbody = $element.find('table > tbody').first();
                                createListItemElement($listitem_data).appendTo($tbody);
                                break;
                            default:
                                var $row = $(`#${$rowid}`);
                                $input_dynamic_form = $row.find('input[type="hidden"]').first();

                                $.each($input_dynamic_form_field, function (index, val) {
                                    if (val == 'Required' || val == 'IsInput' || val == 'IsOutput' || val == 'IsHidden') {
                                        var tempVal1 = $(`#dynamic_form input[name=${val}]`).prop("checked");
                                        $input_dynamic_form.attr(`data-${val}`, tempVal1);
                                        $row.find(`td.${val}`).text(tempVal1 == true ? 'Yes' : 'No');
                                    } else if (val == 'FieldType') {
                                        var tempVal2 = $(`#dynamic_form select[name=${fieldTypeList}]`).val();
                                        $input_dynamic_form.attr(`data-${val}`, tempVal2);
                                        $row.find(`td.${val}`).text(tempVal2);
                                    } else {
                                        var tempVal3 = $.trim($(`#dynamic_form input[name=${val}]`).val());
                                        $input_dynamic_form.attr(`data-${val}`, tempVal3);
                                        $row.find(`td.${val}`).text(tempVal3);
                                    }
                                });
                        }
                        break;
                    default:
                        $.each($input_dynamic_form_field, function (index, val) {
                            if (val == 'FieldType') {
                                $input_dynamic_form.attr(`data-${val}`, $(`#dynamic_form select[name=${val}]`).val());
                            }
                            else if (val == 'Required' || val == 'IsInput' || val == 'IsOutput' || val == 'IsHidden') {
                                $input_dynamic_form.attr(`data-${val}`, $(`#dynamic_form input[name=${val}]`).prop("checked"));
                            } else if (val == 'DefaultValue') {
                                var $defaultValue = $.trim($(`#dynamic_form input[name=${val}]`).val());
                                $input_dynamic_form.attr(`data-${val}`, $defaultValue).attr("value", $defaultValue);
                            } else if (val == 'Placeholder') {
                                var $placeholder = $.trim($(`#dynamic_form input[name=${val}]`).val());
                                $input_dynamic_form.attr(`data-${val}`, $placeholder).attr("placeholder", $placeholder);
                            } else if (val == 'LableName') {
                                var $labelText = $.trim($(`#dynamic_form input[name=${val}]`).val());
                                $element.find("span[name='Lable']").text($labelText);
                                $input_dynamic_form.attr(`data-${val}`, $labelText);
                            } else if (val == 'LableNameMM') {
                                var $labelMMText = $.trim($(`#dynamic_form input[name=${val}]`).val());
                                var labelMM = '';
                                if ($labelMMText) {
                                    labelMM = `(${$labelMMText})`;
                                }
                                $element.find("span[name='LableMM']").text(labelMM);
                                $input_dynamic_form.attr(`data-${val}`, $labelMMText);
                            } else {
                                $input_dynamic_form.attr(`data-${val}`, $.trim($(`#dynamic_form input[name=${val}]`).val()));
                            }
                        });
                }
        }

        $('a[href="#AddField"]').click();
        disableEditableForm();
    }
});

//CheckFieldName
jQuery.validator.addMethod("CheckFieldName",
    function (value, element, param) {
        var fieldName = $(element).val();

        var $edit = $('#EditField');
        var edit_id = $edit.attr("data-id");

        var listItems = $("#droppable li");
        var isDuplicate = false;
        listItems.each(function (idx, li) {
            var $element = $(li);
            var ele_id = $element.attr("id");
            if (edit_id != ele_id) {
                var inputType = $element.attr('data-type');
                var $input_dynamic_form;
                switch (inputType) {
                    case 'Textbox':
                        $input_dynamic_form = $element.find('input[type="text"]').first();
                        break;
                    case 'Number':
                        $input_dynamic_form = $element.find('input[type="number"]').first();
                        break;
                    case 'Selectbox':
                        $input_dynamic_form = $element.find('select').first();
                        break;
                    case 'List':
                        $input_dynamic_form = $element.find('table > thead > tr:first > input[type="hidden"]').first();
                        break;
                }
                if (fieldName == $input_dynamic_form.attr('data-FieldName')) {
                    isDuplicate = true;
                    return false;
                }
            }
        });
        return !isDuplicate;
    }, 'Duplicate Field Name!');

//Check Diff Account No
$.validator.addMethod("NotEqualTo", function (value, element, param) {
    var target = $(param);
    if (value) return $('input[name="IsFix"]:checked').val() != 'FixAmount' || $('input[type=checkbox][name=DiffChargesAccount]').is(":checked") != true || value != target.val();
    else return this.optional(element);
}, "Charges Account No. and Credit Account No. should not be the same!");

//checkDuplicateFieldName
function checkDuplicateFieldName() {
    var listItems = $("#droppable li");
    var isDuplicate = false;
    var inputElements = [];
    listItems.each(function (idx, li) {
        var $element = $(li);
        // var id = $li.attr("id");
        var inputType = $element.attr('data-type');
        var $input_dynamic_form;
        switch (inputType) {
            case 'Textbox':
                $input_dynamic_form = $element.find('input[type="text"]').first();
                break;
            case 'Number':
                $input_dynamic_form = $element.find('input[type="number"]').first();
                break;
            case 'Selectbox':
                $input_dynamic_form = $element.find('select').first();
                break;
            case 'List':
                $input_dynamic_form = $element.find('table > thead > tr:first > input[type="hidden"]').first();
                break;
        }
        var currElement = $input_dynamic_form.attr('data-FieldName');
        if ($.inArray(currElement, inputElements) != -1) {
            isDuplicate = true;
            return false;
        } else {
            inputElements.push(currElement);
        }
    });
    return isDuplicate;
}

//nextTab
function nextTab(elem) {
    $(elem).next().find('a[data-toggle="tab"]').click();
}

//prevTab
function prevTab(elem) {
    $(elem).prev().find('a[data-toggle="tab"]').click();
}

//createLiElement
function createLiElement(val) {
    var uid = function () {
        // Math.random should be unique because of its seeding algorithm.
        // Convert it to base 36 (numbers + letters), and grab the first 9 characters
        // after the decimal.
        return '_' + Math.random().toString(36).substr(2, 9);
    };

    var isDefault = checkDefaultField(val.FieldName, val.FieldType);

    var $li = $("<li>")
        .addClass("ui-corner-all list-group-item clearfix droppable-form ui-selectee")
        .attr("id", uid)
        .attr("data-type", val.FieldType)
        .attr("data-isdefault", isDefault)
        .attr("title", "Click to select.");

    var $remove = $("<div>")
        .addClass("btn btn-danger btn-xs pull-right remove-item")
        .addClass("remove-field");
    $remove.append(createSpanWithIcon("times", "Click to remove."));

    var $reorder = $("<div>")
        .addClass("btn btn-warning btn-xs pull-right sort-item")
        .addClass("sort-field");
    $reorder.append(createSpanWithIcon("sort", "Click and Drag to reorder."));

    return $li.append($remove).append($reorder).append(createFormField(val));
}

//check default field
function checkDefaultField(fieldName, fieldType) {
    return defaultField.some(function (el) {
        return el.FieldName == fieldName && el.FieldType == fieldType;
    });
}

//createSpanWithIcon
function createSpanWithIcon(className, title) {
    return $("<span>")
        .addClass(`fa fa-${className}`)
        .attr('title', title);
}

//createFormField
function createFormField(inputData) {
    var $wrapper = $("<div>")
        .addClass("form-group");

    var $label = $("<label>")
        .addClass("label-name")
        .attr("for", inputData.FieldName)
        .appendTo($wrapper);

    $("<span>")
        .attr("name", "Lable")
        .text(inputData.LableName)
        .appendTo($label);
    var labelMM = '';
    if (inputData.LableNameMM) {
        labelMM = `(${inputData.LableNameMM})`;
    }
    $("<span>")
        .attr("name", "LableMM")
        .text(labelMM)
        .appendTo($label);

    switch (inputData.FieldType) {
        case 'List':
            var $table = createListElement(inputData);
            $table.appendTo($wrapper);
            break;
        default:
            var $input = createFormElement(inputData, inputData.FieldType);
            $input.appendTo($wrapper);
    }
    return $wrapper;
}

//createListElement
function createListElement(inputData) {
    var $table_wrapper = $('<div>')
        .addClass("table-responsive");
    var $table = $('<table>')
        .addClass("table")
        .addClass("table-bordered")
        .appendTo($table_wrapper);

    var $thead = $("<thead>").appendTo($table);
    var $title_row = $("<tr>").addClass("text-nowrap").appendTo($thead);
    var $input_hidden = createFormElement(inputData, inputData.FieldType);
    $input_hidden.appendTo($title_row);
    $.each(tableTitle, function (index, val) {
        $("<td>").text(val).appendTo($title_row);
    });

    var $tbody = $("<tbody>").appendTo($table);
    $.each(inputData.Children, function (index, val) {
        createListItemElement(val).appendTo($tbody);
    });

    var $tfoot = $("<tfoot>").appendTo($table);
    var $add_row = $("<tr>").appendTo($tfoot);
    var $td = $("<td>")
        .addClass("text-center")
        .attr("colspan", 9)
        .appendTo($add_row);
    var $span = $("<span>")
        .addClass("add-table-row")
        .addClass("btn btn-primary")
        .attr("title", "Click to add.")
        .text(" Add New Item")
        .appendTo($td);
    $("<span>")
        .addClass("fa fa-plus-square-o")
        .prependTo($span);
    return $table_wrapper;
}

//createListItemElement
function createListItemElement(val) {
    var uid = function () {
        // Math.random should be unique because of its seeding algorithm.
        // Convert it to base 36 (numbers + letters), and grab the first 9 characters
        // after the decimal.
        return '__' + Math.random().toString(36).substr(2, 9);
    };
    var $data_row = $("<tr>").attr("id", uid);
    var $input_hidden = createFormElement(val, 'ListItem');
    $input_hidden.appendTo($data_row);

    $.each(ListItemFormField, function (index, Listval) {
        if (Listval == 'IsInput' || Listval == 'IsOutput' || Listval == 'IsHidden') {
            var tempVal = val[`${Listval}`];
            $("<td>").addClass(Listval).text(tempVal == true ? 'Yes' : 'No').appendTo($data_row);
        } else if (Listval == 'Required') {
            if (val.Attributes) {
                var tempArrr = val.Attributes;
                var tempVal1 = tempArrr[`${Listval}`];
                $("<td>").addClass(Listval).text(tempVal1 == true ? 'Yes' : 'No').appendTo($data_row);
            }
        } else if (Listval == 'MaxLength' || Listval == 'MinLength') {
            if (val.Attributes) {
                var tempArrr1 = val.Attributes;
                $("<td>").addClass(Listval).text(tempArrr1[`${Listval}`]).appendTo($data_row);
            }
        } else {
            $("<td>").addClass(Listval).text(val[`${Listval}`]).appendTo($data_row);
        }
    });

    var $action = $("<td>").appendTo($data_row);
    $("<span>")
        .addClass("fa fa-pencil-square-o")
        .addClass("edit-table-row")
        .attr("title", "Click to edit.")
        .appendTo($action);
    $("<span>")
        .text(" | ")
        .appendTo($action);
    $("<span>")
        .addClass("fa fa-trash-o")
        .addClass("delete-table-row")
        .attr("title", "Click to delete.")
        .appendTo($action);
    return $data_row;
}

//createFormElement
function createFormElement(inputData, inputType) {
    var $input_dynamic_form, $input_dynamic_form_field;
    switch (inputType) {
        case 'Textbox':
            $input_dynamic_form = $("<input>").attr("type", "text").addClass("form-control");
            $input_dynamic_form_field = TextFormField;
            break;
        case 'Number':
            $input_dynamic_form = $("<input>").attr("type", "number").addClass("form-control");
            $input_dynamic_form_field = NumberFormField;
            break;
        case 'Selectbox':
            $input_dynamic_form = $("<select>").addClass("form-control");
            $input_dynamic_form_field = DropdownFormField;
            break;
        case 'List':
            $input_dynamic_form = $("<input>").attr("type", "hidden");
            $input_dynamic_form_field = ListFormField;
            break;
        case 'ListItem':
            $input_dynamic_form = $("<input>").attr("type", "hidden");
            $input_dynamic_form_field = ListItemFormField;
            break;
    }

    $.each($input_dynamic_form_field, function (index, val) {
        if (val == 'Required' || val == 'MaxLength' || val == 'MinLength') {
            if (inputData.Attributes) {
                $input_dynamic_form.attr(`data-${val}`, inputData.Attributes[`${val}`]);
            }
        } else if (val == 'DefaultValue') {
            var $defaultValue = inputData[`${val}`];
            $input_dynamic_form.attr(`data-${val}`, $defaultValue).attr("value", $defaultValue);
        } else if (val == 'Placeholder') {
            var $placeholder = inputData[`${val}`];
            $input_dynamic_form.attr(`data-${val}`, $placeholder).attr("placeholder", $placeholder);
        } else {
            $input_dynamic_form.attr(`data-${val}`, inputData[`${val}`]);
        }
    });

    return $input_dynamic_form;
}

//disableEditableForm
function disableEditableForm() {
    $('form.editable-form div.form-group').hide();
    $("form.editable-form").css("display", "none");
    $('#droppable .ui-selected').removeClass('ui-selected')
    $('a[href="#EditField"]').closest('li').removeClass('active').addClass('disabled');
    //$('a[href="#addfield"]').closest('li').addClass('active');
    //$('a[href="#AddField"]').click();
}

//readURL
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#image')
                .attr('src', e.target.result)
                .width(100)
                .height(100);
        };

        reader.readAsDataURL(input.files[0]);
        $('.field-validation-error').empty();
    }
}

//save
function SaveToServer(model, biller_Fields, $this, callback) {
    var form = $('#biller_form');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    var formData = new FormData();
    formData.append("Image", $('#Image')[0].files[0]);
    formData.append("model", JSON.stringify(model));
    formData.append("biller_Fields", JSON.stringify(biller_Fields));
    formData.append("__RequestVerificationToken", token);

    $.ajax({
        url: "/Biller/SaveBillerRegistration",
        type: "POST",
        cache: false,
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: formData,
        async: true,
        dataType: "json"
    }).done(function (response) {
        showAlert(response.title, response.message, billerListUrl);
    }).fail(
        function (xhr, ajaxError, thrown) {
            if (xhr.status == 302) {
                window.location.href = "/Admin/Login";
            } else {
                var errorData = $.parseJSON(xhr.responseText);
                var errorMessages = [];
                for (var key in errorData) {
                    errorMessages.push(errorData[key]);
                }
                showAlert(thrown, errorMessages.join("<br />"));
            }
            //function(xhr, textStatus, exceptionThrown) {
            //var errorData = $.parseJSON(xhr.responseText);
            //var errorMessages = [];
            //for (var key in errorData) {
            //    errorMessages.push(errorData[key]);
            //}
            //showAlert(exceptionThrown, errorMessages.join("<br />"));
        }).always(function () {
            callback($this);
        });
}

//update
function UpdateToServer(id, model, biller_Fields, $this, callback) {
    var form = $('#biller_form');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    var formData = new FormData();
    formData.append("id", id);
    formData.append("Image", $('#Image')[0].files[0]);
    formData.append("model", JSON.stringify(model));
    formData.append("biller_Fields", JSON.stringify(biller_Fields));
    formData.append("__RequestVerificationToken", token);

    $.ajax({
        url: "/Biller/UpdateBillerRegistration",
        type: "POST",
        cache: false,
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: formData,
        async: true,
        dataType: "json"
    }).done(function (response) {
        showAlert(response.title, response.message, billerListUrl);
    }).fail(function (xhr, ajaxError, thrown) {
        if (xhr.status == 302) {
            window.location.href = "/Admin/Login";
        } else {
            var errorData = $.parseJSON(xhr.responseText);
            var errorMessages = [];
            for (var key in errorData) {
                errorMessages.push(errorData[key]);
            }
            showAlert(thrown, errorMessages.join("<br />"));
        }
        //function (xhr, textStatus, exceptionThrown) {
        //var errorData = $.parseJSON(xhr.responseText);
        //var errorMessages = [];
        //for (var key in errorData) {
        //    errorMessages.push(errorData[key]);
        //}
        //showAlert(exceptionThrown, errorMessages.join("<br />"));
    }).always(function () {
        callback($this);
    });
}

//AutofillforTesting
function AutofillforTesting() {
    $('#Name').val('test11');
    $('#BillerCode').val('test11');
    $('#FixAmount').val('1000');
    $('#CreditAccountNo').val('123');
    $('#BillerType').val('GiftCard');
    $('#Currency').val('EUR');
}

//resetLoading
function resetLoading($this) {
    $this.button('reset');
}

//bind View To Model
function ViewToModel() {
    var isFix, FixAmount, ChargesCode, DiscountAmount, DiscountPercentage;
    switch ($('input[name="IsFix"]:checked').val()) {
        case 'FixAmount':
            isFix = 0;
            FixAmount = $('#FixAmount').val();
            break;
        case 'ChargesCode':
            isFix = 1;
            ChargesCode = $('#ChargesCode').val();
            break;
        case 'Discount':
            isFix = 2;
            switch ($('input[name="IsFixed"]:checked').val()) {
                case 'Percentage':
                    DiscountPercentage = $('#Discount').val();
                    break;
                case 'FixedAmount':
                    DiscountAmount = $('#Discount').val();
                    break;
                default:
            }
            break;
        default:
    }

    var creditAccountNo = $('#CreditAccountNo').val();
    var chargesAccountNo;
    switch ($('input[type=checkbox][name=DiffChargesAccount]').is(":checked")) {
        case true:
            chargesAccountNo = $('#ChargesAccountNo').val();
            break;
        default:
            chargesAccountNo = creditAccountNo;
    }

    var data = {
        ID: $('#BillerId').val(),
        Name: $.trim($('#Name').val()),
        BillerCode: $.trim($('#BillerCode').val()),
        IsApiIntegrate: $('#IsApiIntegrate').is(":checked"),
        ChargesCode: ChargesCode,
        ChargesAccountNo: $.trim(chargesAccountNo),
        CreditAccountNo: $.trim(creditAccountNo),
        isFixRate: isFix,
        ChargesAmount: FixAmount,
        DiscountAmount: DiscountAmount,
        DiscountPercentage: DiscountPercentage,
        BillerType: $('#BillerType').val(),
        Currency: $('#Currency').val()
    };

    return data;
}

//bind model to view
function ModelToView(biller, billerFields, isEdit) {
    //Main
    $('#Name').val(biller.Name);
    $('#BillerCode').val(biller.BillerCode);
    $("#IsApiIntegrate").prop("checked", biller.IsApiIntegrate);
    $('#CreditAccountNo').val(biller.CreditAccountNo);
    //$('#image').attr("src", biller.ImagePath);
    //$('#image').load(biller.ImagePath, function (response, status, xhr) {
    //    if ($('#Image')[0].files.length > 0) {
    //        //DO NOTHING
    //    }
    //    else if (status == "error") {
    //        $(this).attr('src', '/Images/mini-side-bar-logo.png')
    //            .attr('data-required', true);
    //    }
    //    else {
    //        $(this).attr('src', biller.ImagePath)
    //            .attr('data-required', false);
    //    }
    //});

    switch (biller.isFixRate) {
        case 0:
            $("input[name=IsFix][value='FixAmount']").prop("checked", true);
            $('#FixAmount').val(biller.ChargesAmount);
            if (biller.CreditAccountNo == biller.ChargesAccountNo) {
                $("input[type=checkbox][name=DiffChargesAccount]").prop("checked", false);
                $('.DiffChargesAcc').hide();
            } else {
                $("input[type=checkbox][name=DiffChargesAccount]").prop("checked", true);
                $('#ChargesAccountNo').val(biller.ChargesAccountNo);
                $('.DiffChargesAcc').show();
            }
            $('.Fix-Amount').show();
            $('.Charges-Code').hide();
            $('.Discount').hide();
            break;
        case 1:
            $("input[name=IsFix][value='ChargesCode']").prop("checked", true);
            $('#ChargesCode').val(biller.ChargesCode);
            $('.Fix-Amount').hide();
            $('.Charges-Code').show();
            $('.Discount').hide();
            break;
        case 2:
            $("input[name=IsFix][value='Discount']").prop("checked", true);
            if (biller.DiscountPercentage) {
                $("input[name=IsFixed][value='Percentage']").prop("checked", true);
                $('#Discount').val(biller.DiscountPercentage);
            } else if (biller.DiscountAmount) {
                $("input[name=IsFixed][value='FixedAmount']").prop("checked", true);
                $('#Discount').val(biller.DiscountAmount);
            }
            $('.Fix-Amount').hide();
            $('.Charges-Code').hide();
            $('.Discount').show();
            break;
        default:
    }

    $('#BillerType').val(biller.BillerType);
    $('#Currency').val(biller.Currency);

    if (biller.ImagePath) {
        $('#image').attr("src", `data:image/png;base64,${biller.ImagePath}`);
        $('#Image').attr('data-required', false);
    }

    $.each(billerFields, function (index, val) {
        var $item = createLiElement(val);
        $('#droppable').append($item);
    });
}

//add status
function ChangeToAddStatus() {
    $('.back-step').parents('li').hide();
}

//update status
function ChangeToUpdateStatus() {
    $('#Register').text('Update');
    $('#Register').val('Update');
    //$('.back-step').parents('li').show();
}

//detail status
function ChangeToDetailStatus() {
    $('#Register').text('Update');
    $('#Register').val('Detail');
    $('#Register').prop('disabled', true);
    //$('.back-step').parents('li').show();

    //disable input
    //main
    $("#Name").prop('disabled', true);
    $("#BillerCode").prop('disabled', true);
    $("#IsApiIntegrate").prop('disabled', true);
    $("input[type=checkbox][name=DiffChargesAccount]").prop('disabled', true);
    $("input[name=IsFix]").prop('disabled', true);
    $("input[name=IsFixed]").prop('disabled', true);
    $("#FixAmount").prop('disabled', true);
    $("#ChargesAccountNo").prop('disabled', true);
    $("#ChargesCode").prop('disabled', true);
    $("#Discount").prop('disabled', true);
    $("#CreditAccountNo").prop('disabled', true);
    $("#BillerType").prop('disabled', true);
    $("#Currency").prop('disabled', true);
    $("#Image").prop('disabled', true);

    //$("#droppable").selectable("option", "disabled", true);
    $("#droppable").sortable("option", "disabled", true);
    $("#draggable li").draggable("option", "disabled", true);
}

//GetBillerField
function GetBillerField($input_dynamic_form, $input_dynamic_form_field, idx, inputType = '') {
    var jsonElementData = {};
    $.each($input_dynamic_form_field, function (index, val) {
        if (val == 'Required' || val == 'MaxLength' || val == 'MinLength') {
            var $attributes = jsonElementData["Attributes"];
            if (!$attributes) {
                $attributes = {};
            }
            $attributes[`${val}`] = $.trim($input_dynamic_form.attr(`data-${val}`));
            jsonElementData["Attributes"] = $attributes;
        } else {
            jsonElementData[`${val}`] = $.trim($input_dynamic_form.attr(`data-${val}`));
        }
    });
    jsonElementData['SortOrder'] = idx + 1;
    return jsonElementData;
}

//GetBillerFields
function GetBillerFields() {
    var listItems = $("#droppable li");
    var jsonElementsData = [];
    listItems.each(function (idx, li) {
        var jsonElementData = {};
        var $element = $(li);
        // var id = $li.attr("id");
        var inputType = $element.attr('data-type');
        var $input_dynamic_form;
        switch (inputType) {
            case 'Textbox':
                $input_dynamic_form = $element.find('input[type="text"]').first();
                jsonElementData = GetBillerField($input_dynamic_form, TextFormField, idx, inputType);
                break;
            case 'Number':
                $input_dynamic_form = $element.find('input[type="number"]').first();
                jsonElementData = GetBillerField($input_dynamic_form, NumberFormField, idx, inputType);
                break;
            case 'Selectbox':
                $input_dynamic_form = $element.find('select').first();
                jsonElementData = GetBillerField($input_dynamic_form, DropdownFormField, idx, inputType);
                break;
            case 'List':
                $input_dynamic_form = $element.find('table > thead > tr:first > input[type="hidden"]').first();
                jsonElementData = GetBillerField($input_dynamic_form, ListFormField, idx, inputType);

                var jsonChildren = [];
                var listRows = $element.find('table > tbody > tr');
                listRows.each(function (index, row) {
                    var $table_row = $(row).find('input[type="hidden"]').first();
                    var jsonListItemData = GetBillerField($table_row, ListItemFormField, index);
                    jsonChildren.push(jsonListItemData);
                });
                jsonElementData["Children"] = jsonChildren;
                break;
        }
        jsonElementsData.push(jsonElementData);
    });
    return jsonElementsData;
}

//show alert message
function showAlert(title, message, url = '') {
    bootbox.alert({
        size: "small",
        title: title,
        message: message,
        callback: function () {
            if (url) {
                window.location.href = url;
            }
        }
    })
}

//show message
function showAlertWithReload(title, message) {
    bootbox.alert({
        size: "small",
        title: title,
        message: message,
        callback: function () {
            location.reload();
        }
    })
}