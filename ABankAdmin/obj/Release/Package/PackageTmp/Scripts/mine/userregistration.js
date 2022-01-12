//
var MenuInfo = [];
var AccInfo = [];
var menutable;
var acctable;
var isAccTitleChecked;

const getMenuUrl = '/CorporateUserRequest/GetMenu';
const getAccountUrl = '/CorporateUserRequest/GetAccount';

//for getting Corporate Limit
const getCorporateLimitUrl = '/CorporateUserRequest/GetCorporateLimit';

const reqUserListUrl = '/CorporateUserRequest/RequestUserList';
const reqGetUserRegUrl = '/CorporateUserRequest/GetReqUserReg';
const reqGetCorpUserUrl = '/CorporateUserRequest/GetReqCorporateData';
const reqGetMenuInfoUrl = '/CorporateUserRequest/GetReqMenuInfo';
const reqGetAccountInfoUrl = '/CorporateUserRequest/GetReqAccountInfo';
const reqUpdateUserRegUrl = '/CorporateUserRequest/UpdateReqCorporateUserReg';

const userListUrl = '/CorporateUserRequest/CorporateUserList';
const getUserRegUrl = '/CorporateUserRequest/GetUserReg';
const getCorpUserUrl = '/CorporateUserRequest/GetCorporateData';
const getMenuInfoUrl = '/CorporateUserRequest/GetMenuInfo';
const getAccountInfoUrl = '/CorporateUserRequest/GetAccountInfo';
const updateUserRegUrl = '/CorporateUserRequest/UpdateCorporateUserReg';

function initialize() {
    var id = $('#CopRegId').val();
    $('.Maker').hide();
    $('.Checker').hide();
    //for update and detail
    switch ($('#CopRegStatus').val()) {
        case '2':
            //get user data
            GetUserRegData(id, false, false);
            //update
            ChangeToUpdateStatus(false);
            break;
        case '3':
            //get user data
            GetUserRegData(id, false, true);
            //detail
            ChangeToDetailStatus();
            break;
        case '4':
            //get user data
            GetUserRegData(id, true, false);
            //update
            ChangeToUpdateStatus(true);
            break;
        case '5':
            //get user data
            GetUserRegData(id, true, true);
            //detail
            ChangeToDetailStatus();
            break;
        default:
            //autoComplete
            InitializeCorporateUser();
            //get menu data
            //GetMenuInfoData(getMenuUrl, '');
            //show or hide role menu
            showhideRole(true);
    }
}

$(function () {
    initialize();

    $(function () {
        // Set up the number formatting with thousand separator
        $('.number-separator').number(true, 0);
    });
});

function InitializeCorporateUser() {
    //auto complete textbox for CorporateUser
    $("#CorporateUserId").autocomplete({
        minLength: 1,
        delay: 100,
        source: function (request, response) {
            //remove id
            $("#CorpID").val('');
            $("#CorporateID").val('');
            $("#CIFID").val('');
            $.ajax({
                url: "/CorporateUserRequest/GetCorporateData",
                type: "POST",
                data: {
                    term: request.term
                },
                success: function (data) {
                    response(data);
                },
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
                //    showAlertWithReload(exceptionThrown, errorMessages.join("<br />"));
                //    $("#CorporateUserId").removeClass("ui-autocomplete-loading");
                //}
            });
        },
        focus: function (event, ui) {
            $("#CorporateUserId").val(ui.item.CompanyName);
            return false;
        },
        select: function (event, ui) {
            $("#CorporateUserId").val(ui.item.CompanyName);
            $("#CorpID").val(ui.item.CorpID);
            $("#CorporateID").val(ui.item.CorporateID);
            $("#CIFID").val(ui.item.CIFID);

            GetAccountInfoData(getAccountUrl, ui.item.CIFID, '');
            getCorporateLimit(ui.item.CorpID);//to get limit
            return false;
        }
    }).data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li>")
            .append("<span>" + item.CompanyName + " (" + item.CorporateID + ")</span>")
            .appendTo(ul);
    };
}

//get user data by id
function GetUserRegData(id, isReqUser = false, isDetail = false) {
    var actionUrl = isReqUser == true ? reqGetUserRegUrl : getUserRegUrl;
    var accountUrl = isReqUser == true ? reqGetAccountInfoUrl : getAccountInfoUrl;
    var menuUrl = isReqUser == true ? reqGetMenuInfoUrl : getMenuInfoUrl;
    var userId;
    var cifid;
    $.post(actionUrl,
        {
            id: id
        },
        function (response) {
            cifid = response.CIFID;
            if (isReqUser) {
                userId = response.Id;//ReqUserID
            } else {
                userId = response.USERID;//user id
            }
            $('#GetMenuId').val(userId);
            ModelToView(response);
        }).fail(function (xhr, textStatus, exceptionThrown) {
            var errorData = $.parseJSON(xhr.responseText);
            var errorMessages = [];
            for (var key in errorData) {
                errorMessages.push(errorData[key]);
            }
            showAlert(exceptionThrown, errorMessages.join("<br />"), isReqUser == false ? userListUrl : reqUserListUrl);
        }).done(function () {
            GetAccountInfoData(accountUrl, cifid, userId);
            if (isDetail) {
                GetMenuInfoDataForDetail(menuUrl, userId);
            }
            //Get Corporate Limit
            getCorporateLimit($('#CorpID').val());
        });
}

//get menu by id
function GetMenuInfoData(actionUrl, id, $this, callback, callback2) {
    $.post(actionUrl,
        {
            data: GetRoleData(),
            id: id
        },
        function (response) {
            MenuInfo = response;
        }).fail(
            function(xhr, ajaxError, thrown) {
                if (xhr.status == 302) {
                    window.location.href = "/Admin/Login";
                }
            //function(xhr, textStatus, exceptionThrown) {
            //var errorData = $.parseJSON(xhr.responseText);
            //var errorMessages = [];
            //for (var key in errorData) {
            //    errorMessages.push(errorData[key]);
            //}
            //showAlertWithReload(exceptionThrown, errorMessages.join("<br />"));
        }).done(function () {
            showMenuDataOnTable();
            showMenu();
            callback();
        }).always(function () {
            callback2($this);
        });
}

//get menu by id
function GetMenuInfoDataForDetail(actionUrl, id) {
    $.post(actionUrl,
        {
            data: GetRoleData(),
            id: id
        },
        function (response) {
            MenuInfo = response;
        }).fail(function (xhr, textStatus, exceptionThrown) {
            var errorData = $.parseJSON(xhr.responseText);
            var errorMessages = [];
            for (var key in errorData) {
                errorMessages.push(errorData[key]);
            }
            showAlertWithReload(exceptionThrown, errorMessages.join("<br />"));
        }).done(function () {
            showMenuDataOnTable();
            showMenu();
        });
}

//construct data for menu info
function GetRoleData() {
    var IsAdmin = false;
    var IsTransaction = false;
    var IsAdminMaker = false;
    var IsAdminApprover = false;
    var IsMaker = false;
    var IsViewer = false;
    var IsApprover = false;

    switch ($('input[name="IsAdmin"]:checked').val()) {
        case 'Transaction':
            IsTransaction = true;
            IsMaker = $('#IsMaker').is(":checked");
            IsViewer = $('#IsViewer').is(":checked");
            IsApprover = $('#IsApprover').is(":checked");
            break;
        default:
            IsAdmin = true;
            IsAdminMaker = $('#IsAdminMaker').is(":checked");
            IsAdminApprover = $('#IsAdminApprover').is(":checked");
    }

    var data = {
        IsAdmin: IsAdmin,
        IsTransaction: IsTransaction,
        IsAdminMaker: IsAdminMaker,
        IsAdminApprover: IsAdminApprover,
        IsMaker: IsMaker,
        IsViewer: IsViewer,
        IsApprover: IsApprover
    };

    return data;
}

//get account by id
function GetAccountInfoData(actionUrl, CIFID, id) {
    $.post(actionUrl,
        {
            CIFID: CIFID,
            id: id
        },
        function (response) {
            AccInfo = response;
        }).fail(
            function(xhr, ajaxError, thrown) {
                if (xhr.status == 302) {
                    window.location.href = "/Admin/Login";
                }
            //function(xhr, textStatus, exceptionThrown) {
            //var errorData = $.parseJSON(xhr.responseText);
            //var errorMessages = [];
            //for (var key in errorData) {
            //    errorMessages.push(errorData[key]);
            //}
            //showAlert(exceptionThrown, errorMessages.join("<br />"));
        }).done(function () {
            showAccDataOnTable();
        });
}

//show menu list
function showMenuDataOnTable() {
    //destroy table
    if (menutable) {
        menutable.clear().destroy();
    }
    $('#menuTable').empty();

    var status = $('#CopRegStatus').val();

    //disable for detail
    var isDisable = status == '3' || status == '5' ? 'disabled' : '';
    /*disable datatable warnings*/
    $.fn.dataTable.ext.errMode = 'none';
    menutable = $('#menuTable').DataTable({
        //destroy: true,
        responsive: true,
        processing: true, // for show progress bar
        searching: false,
        paging: false,
        ordering: true,
        info: false,
        data: MenuInfo,
        order: [[0, 'asc']],
        columnDefs: [
            {
                targets: [0],
                searchable: false,
                orderable: true,
                class: "wrapok"
            },
            {
                targets: [1],
                searchable: false,
                orderable: false,
                // Create checkbox
                "render": function (data, type, row, meta) {
                    var adminmakercheckbox = '';
                    if (row.IsAdminMakerEnable) {
                        var is_checked = data == true ? "checked" : "";
                        adminmakercheckbox = '<input type="checkbox" class="checkbox adminmakercheckbox" ' + is_checked + ' ' + isDisable + ' />';
                    }
                    return adminmakercheckbox;
                }
            },
            {
                targets: [2],
                searchable: false,
                orderable: false,
                // Create checkbox
                "render": function (data, type, row, meta) {
                    var adminapprovercheckbox = '';
                    if (row.IsAdminApproverEnable) {
                        var is_checked = data == true ? "checked" : "";
                        adminapprovercheckbox = '<input type="checkbox" class="checkbox adminapprovercheckbox" ' + is_checked + ' ' + isDisable + ' />';
                    }
                    return adminapprovercheckbox;
                }
            },
            {
                targets: [3],
                searchable: false,
                orderable: false,
                // Create checkbox
                "render": function (data, type, row, meta) {
                    var makercheckbox = '';
                    if (row.IsMakerEnable) {
                        var is_checked = data == true ? "checked" : "";
                        makercheckbox = '<input type="checkbox" class="checkbox makercheckbox" ' + is_checked + ' ' + isDisable + ' />';
                    }
                    return makercheckbox;
                }
            },
            {
                targets: [4],
                searchable: false,
                orderable: false,
                // Create checkbox
                "render": function (data, type, row, meta) {
                    var viewercheckbox = '';
                    if (row.IsViewerEnable) {
                        var is_checked = data == true ? "checked" : "";
                        viewercheckbox = '<input type="checkbox" class="checkbox viewercheckbox" ' + is_checked + ' ' + isDisable + ' />';
                    }
                    return viewercheckbox;
                }
            },
            {
                targets: [5],
                searchable: false,
                orderable: false,
                // Create checkbox
                "render": function (data, type, row, meta) {
                    var approvercheckbox = '';
                    if (row.IsApproverEnable) {
                        var is_checked = data == true ? "checked" : "";
                        approvercheckbox = '<input type="checkbox" class="checkbox approvercheckbox" ' + is_checked + ' ' + isDisable + ' />';
                    }
                    return approvercheckbox;
                }
            }
        ],
        columns: [
            { data: 'MenuName', name: 'MenuName', title: 'Menu Name', autoWidth: true },
            { data: 'IsAdminMaker', name: 'IsAdminMaker', title: 'AdminMaker', autoWidth: true, visible: false },
            { data: 'IsAdminApprover', name: 'IsAdminApprover', title: 'AdminApprover', autoWidth: true, visible: false },
            { data: 'IsMaker', name: 'IsMaker', title: 'Maker', autoWidth: true, visible: false },
            { data: 'IsViewer', name: 'IsViewer', title: 'Viewer', autoWidth: true, visible: false },
            { data: 'IsApprover', name: 'IsApprover', title: 'Approver', autoWidth: true, visible: false },
            { data: 'MenuInfoId', name: 'MenuInfoId', title: '', visible: false }
        ]
    });
}

//show account list
function showAccDataOnTable() {
    //destroy data
    if (acctable) {
        acctable.clear().destroy();
    }
    $('#accTable').empty();

    var status = $('#CopRegStatus').val();

    //disable for detail
    var isDisable = status == '3' || status == '5' ? 'disabled' : '';

    //to show/hide checkbox
    var UserTypeRadio = $('input[type=radio][name=IsAdmin]:checked').val();
    var isShow = !(UserTypeRadio == 'Admin');

    //isAccTitleChecked
    var is_AccTitleChecked = isAccTitleChecked == true ? "checked" : "";

    acctable = $('#accTable').DataTable({
        destroy: true,
        responsive: true,
        processing: true, // for show progress bar
        searching: false,
        paging: true,
        ordering: true,
        info: false,
        data: AccInfo,
        order: [[1, 'asc']],
        columnDefs: [
            {
                targets: [0],
                searchable: false,
                orderable: false,
                // Create checkbox
                "render": function (data) {
                    var is_checked = data == true ? "checked" : "";
                    return '<input type="checkbox" class="checkbox acccheckbox" ' + is_checked + ' ' + isDisable + ' />';
                }
            },
            {
                targets: [1, 2],
                searchable: false,
                orderable: true
            }
        ],
        columns: [
            { data: 'IsActive', name: 'IsActive', title: "<input type='checkbox' class='checkbox' id='accTitleChk' " + is_AccTitleChecked + " " + isDisable + " />", autoWidth: true, visible: isShow },
            { data: 'AccountNo', name: 'AccountNo', title: 'Account No', autoWidth: true },
            { data: 'AccountTypeDesc', name: 'AccountTypeDesc', title: 'Account Type Description', autoWidth: true },
            { data: 'AccountInfoId', name: 'AccountInfoId', title: 'AccountInfoId', visible: false }
        ]
    });

    checkAccDataStatus();
}

//for stepper form
$(document).ready(function () {
    //Initialize tooltips
    //$('.nav-tabs > li a[title]').tooltip();

    //Wizard
    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
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
    $('body').on('click', '.next-step', function (e) {
        //get status
        var status = $('#CopRegStatus').val();
        var $this = $(this);
        var nextVal = $this.val();
        var IsAdminVal = $('input[name="IsAdmin"]:checked').val();
        var menuId = $('#GetMenuId').val();

        //check status
        if (status == '3' || status == '5') {
            switch (nextVal) {
                case 'Menu':
                    if (IsAdminVal == 'Transaction' && ($('#IsMaker').is(":checked") || $('#IsApprover').is(":checked"))) {
                        gotoNext();
                    } else {
                        gotoAccountInfo();
                    }
                    break;

                case 'Detail':
                    switch (status) {
                        case '3':
                            window.location.href = userListUrl;
                            break;
                        default:
                            window.location.href = reqUserListUrl;
                    }
                    break;

                default:
                    gotoNext();
            }
        } else {
            switch (nextVal) {
                //case 'UserInfo':
                //    $('#UserName').valid();
                //    checkValidData();
                //    break;

                case 'Role':
                    if (!checkRoleInfo()) {
                        showAlert('Error', 'At least one of role item need to check.');
                    } else {
                        //get menu data
                        $this.button('loading');
                        switch (status) {
                            case '2':
                                GetMenuInfoData(getMenuInfoUrl, menuId, $this, gotoNext, resetLoading);
                                break;
                            case '4':
                                GetMenuInfoData(reqGetMenuInfoUrl, menuId, $this, gotoNext, resetLoading);
                                break;
                            default:
                                GetMenuInfoData(getMenuUrl, menuId, $this, gotoNext, resetLoading);
                        }
                    }
                    break;

                case 'Menu':
                    if (!checkMenu()) {
                        showAlert('Error', 'At least one of menu item need to check.');
                    } else {
                        if (IsAdminVal == 'Transaction' && ($('#IsMaker').is(":checked") || $('#IsApprover').is(":checked"))) {
                            gotoNext();
                        } else {
                            gotoAccountInfo();
                        }
                    }
                    break;

                case 'Register':
                    if (validator.valid()) {
                        if (!checkRoleInfo()) {
                            showAlert('Error', 'At least one of role item need to check.');
                        } else if (!checkItemInAcc()) {
                            showAlert('Error', 'At least one of account item need to check.');
                        } else if (!checkMenu()) {
                            showAlert('Error', 'At least one of menu item need to check.');
                        } else {
                            //submit
                            $this.button('loading');
                            var model = ViewToModel();
                            SaveToServer(model, $this, resetLoading);
                        }
                    } else {
                        showAlert('Error', 'Your form has some error! Please check again.');
                    }
                    break;

                case 'Update':
                    if (validator.valid()) {
                        if (!checkMenu()) {
                            showAlert('Error', 'At least one of menu item need to check.');
                        } else if (!checkRoleInfo()) {
                            showAlert('Error', 'At least one of role item need to check.');
                        } else if (!checkItemInAcc()) {
                            showAlert('Error', 'At least one of account item need to check.');
                        } else {
                            //submit
                            $this.button('loading');
                            var model = ViewToModel();
                            var id = $('#CopRegId').val();
                            if (id && status == '2') {
                                UpdateToServer(updateUserRegUrl, id, model, userListUrl, $this, resetLoading);
                            }
                        }
                    } else {
                        showAlert('Error', 'Your form has some error! Please check again.');
                    }
                    break;

                case 'UpdateReq':
                    if (validator.valid()) {
                        if (!checkMenu()) {
                            showAlert('Error', 'At least one of menu item need to check.');
                        } else if (!checkRoleInfo()) {
                            showAlert('Error', 'At least one of role item need to check.');
                        } else if (!checkItemInAcc()) {
                            showAlert('Error', 'At least one of account item need to check.');
                        } else {
                            //submit
                            $this.button('loading');
                            var model = ViewToModel();
                            var id = $('#CopRegId').val();
                            if (id && status == '4') {
                                UpdateToServer(reqUpdateUserRegUrl, id, model, reqUserListUrl, $this, resetLoading);
                            }
                        }
                    } else {
                        showAlert('Error', 'Your form has some error! Please check again.');
                    }
                    break;

                default:
                    checkValidData();
            }
        }
    });

    //previous button click
    $('body').on('click', '.prev-step', function (e) {
        var $active = $('.wizard .nav-tabs li.active');
        var IsAdminVal = $('input[name="IsAdmin"]:checked').val();
        //if (!$active.hasClass('disabled')) {
        //    $active.addClass('disabled');
        //}
        if (($(this).val() == 'Register' || $(this).val() == 'Update' || $(this).val() == 'Detail')
            && (IsAdminVal == 'Admin' || (IsAdminVal == 'Transaction' && !$('#IsMaker').is(":checked") && !$('#IsApprover').is(":checked")))) {
            if (!$active.prev().hasClass('disabled')) {
                $active.prev().addClass('disabled');
            }
            gobacktoMenuInfo($active);
        } else {
            prevTab($active);
        }
    });
});

//check partial form data
function checkValidData() {
    var valid = true;
    var $activeTab = $('.wizard .tab-content .tab-pane.active');
    $activeTab.find("input").each(function () {
        if (!validator.element(this) && valid) {
            valid = false;
        }
    });
    $activeTab.find("select").each(function () {
        if (!validator.element(this) && valid) {
            valid = false;
        }
    });
    if (valid) {
        var $active = $('.wizard .wizard-inner .nav-tabs li.active');
        $active.next().removeClass('disabled');
        nextTab($active);
    }
}

//skip transaction info
function gotoAccountInfo() {
    var $active = $('.wizard .wizard-inner .nav-tabs li.active');
    if (!$active.next().hasClass('disabled')) {
        $active.next().addClass('disabled');
    }
    $active.next().next().removeClass('disabled');
    $($active).next().next().find('a[data-toggle="tab"]').click();
}

function gotoNext() {
    var $active = $('.wizard .wizard-inner .nav-tabs li.active');
    $active.next().removeClass('disabled');
    nextTab($active);
}

function resetLoading($this) {
    $this.button('reset');
}

//skip transaction info
function gobacktoMenuInfo(elem) {
    $(elem).prev().prev().find('a[data-toggle="tab"]').click();
}

//goto next tab
function nextTab(elem) {
    $(elem).next().find('a[data-toggle="tab"]').click();
}

//goto previous tab
function prevTab(elem) {
    $(elem).prev().find('a[data-toggle="tab"]').click();
}

$('document').ready(function () {
    //acc check/uncheck
    $(document).on('change', '#accTitleChk', function (e) {
        if (this.checked) {
            toggleSelectAccData(true);
            isAccTitleChecked = true;
        } else {
            toggleSelectAccData(false);
            isAccTitleChecked = false;
        }
        showAccDataOnTable();
    });

    //check role
    $('input[type=radio][name=IsAdmin]').change(function () {
        //to show and hide Account checkbox
        showAccDataOnTable();
        if (this.value == 'Admin') {
            //show or hide role menu
            showhideRole(true);

            //uncheck menu
            $("#IsMaker").prop("checked", false);
            $("#IsViewer").prop("checked", false);
            $("#IsApprover").prop("checked", false);

            //hide trans menu
            showhideMakerMenu(false);
            showhideCheckerMenu(false);
            showhideApproverMenu(false);
        }
        else if (this.value == 'Transaction') {
            //show or hide role menu
            showhideRole(false);

            //uncheck menu
            $("#IsAdminMaker").prop("checked", false);
            $("#IsAdminApprover").prop("checked", false);

            //hide admin menu
            showhideAdminMakerMenu(false);
            showhideAdminApproverMenu(false);
        }
    });

    //Adminmaker
    $('#IsAdminMaker').change(function () {
        if ($(this).is(":checked")) {
            showhideAdminMakerMenu(true);
        }
        else {
            showhideAdminMakerMenu(false);
        }
    });

    //Adminapprover
    $('#IsAdminApprover').change(function () {
        if ($(this).is(":checked")) {
            showhideAdminApproverMenu(true);
        }
        else {
            showhideAdminApproverMenu(false);
        }
    });

    //maker
    $('#IsMaker').change(function () {
        if ($(this).is(":checked")) {
            showhideMakerMenu(true);
        }
        else {
            showhideMakerMenu(false);
        }
    });

    //checker
    $('#IsViewer').change(function () {
        if ($(this).is(":checked")) {
            showhideCheckerMenu(true);
        }
        else {
            showhideCheckerMenu(false);
        }
    });

    //approver
    $('#IsApprover').change(function () {
        if ($(this).is(":checked")) {
            showhideApproverMenu(true);
        }
        else {
            showhideApproverMenu(false);
        }
    });

    //$('#myTable tbody').on('click', 'tr', function () {
    //    var data = table.row(this).data();
    //    var $this = $(this).children('td:first').children('.menucheckbox');
    //    if (data.IsActive) {
    //        $this.prop("checked", false);
    //        updateMenuData(data.MenuInfoId, false);
    //    } else {
    //        $this.prop("checked", true);
    //        updateMenuData(data.MenuInfoId, true);
    //    }
    //});

    //$('#accTable tbody').on('click', 'tr', function () {
    //    var data = acctable.row(this).data();
    //    var $this = $(this).children('td:first').children('.acccheckbox');
    //    if (data.IsActive) {
    //        $this.prop("checked", false);
    //        updateAccData(data.AccountInfoId, false);
    //    } else {
    //        $this.prop("checked", true);
    //        updateAccData(data.AccountInfoId, true);
    //    }
    //});

    //$(document).on('click', '#menuTable tbody tr', function () {
    //    var data = table.row(this).data();
    //    var $this = $(this).children('td:first').children('.menucheckbox');
    //    if (data.IsActive) {
    //        $this.prop("checked", false);
    //        updateMenuData(data.MenuInfoId, false);
    //    } else {
    //        $this.prop("checked", true);
    //        updateMenuData(data.MenuInfoId, true);
    //    }
    //});

    //check radio by row click in account table
    $(document).on('click', '#accTable tbody tr', function () {
        var status = $('#CopRegStatus').val();
        if (status != '3' && status != '5') {
            var data = acctable.row(this).data();
            var $this = $(this).children('td:first').children('.acccheckbox');
            if (data.IsActive) {
                $this.prop("checked", false);
                updateAccData(data.AccountNo, false);
            } else {
                $this.prop("checked", true);
                updateAccData(data.AccountNo, true);
            }
        }

        checkAccDataStatus();
    });

    //update maker menu
    $(document).on('change', '.adminmakercheckbox', function () {
        var firsttr = $(this).closest('tr');
        var data = menutable.row(firsttr).data();
        if (this.checked) {
            updateAdminMakerMenuData(data.MenuInfoId, true);
        } else {
            updateAdminMakerMenuData(data.MenuInfoId, false);
        }
    });

    //update approver menu
    $(document).on('change', '.adminapprovercheckbox', function () {
        var firsttr = $(this).closest('tr');
        var data = menutable.row(firsttr).data();
        if (this.checked) {
            updateAdminApproverMenuData(data.MenuInfoId, true);
        } else {
            updateAdminApproverMenuData(data.MenuInfoId, false);
        }
    });

    //update maker menu
    $(document).on('change', '.makercheckbox', function () {
        var firsttr = $(this).closest('tr');
        var data = menutable.row(firsttr).data();
        if (this.checked) {
            updateMakerMenuData(data.MenuInfoId, true);
        } else {
            updateMakerMenuData(data.MenuInfoId, false);
        }
    });

    //update checker menu
    $(document).on('change', '.viewercheckbox', function () {
        var firsttr = $(this).closest('tr');
        var data = menutable.row(firsttr).data();
        if (this.checked) {
            updateCheckerMenuData(data.MenuInfoId, true);
        } else {
            updateCheckerMenuData(data.MenuInfoId, false);
        }
    });

    //update approver menu
    $(document).on('change', '.approvercheckbox', function () {
        var firsttr = $(this).closest('tr');
        var data = menutable.row(firsttr).data();
        if (this.checked) {
            updateApproverMenuData(data.MenuInfoId, true);
        } else {
            updateApproverMenuData(data.MenuInfoId, false);
        }
    });

    //$(document).on('change', '.acccheckbox', function () {
    //    var firsttr = $(this).closest('tr');
    //    var data = acctable.row(firsttr).data();
    //    if (this.checked) {
    //        updateAccData(data.AccountInfoId, true);
    //    } else {
    //        updateAccData(data.AccountInfoId, false);
    //    }
    //});

    //$("#CorporateUserId").change(function () {
    //    if (this.value) {
    //        var CIFID = $('#Cifid').val();
    //        var CorporateID = $('#CorporateId').val();
    //        GetAccountInfoData(CIFID);
    //        GetMenuInfoData(CIFID);
    //    }
    //});
    //$(".menucheckbox").change(function () {
    //    debugger
    //    var firsttr = $(this).closest('tr');
    //    var data = table.row(firsttr).data();
    //    if ($(this).is(":checked")) {
    //        updateMenuData(data.MenuInfoId, true);
    //    } else {
    //        updateMenuData(data.MenuInfoId, false);
    //    }
    //});

    //$(".acccheckbox").change(function () {
    //    debugger
    //    var firsttr = $(this).closest('tr');
    //    var data = acctable.row(firsttr).data();
    //    if ($(this).is(":checked")) {
    //        updateAccData(data.AccountInfoId, true);
    //    } else {
    //        updateAccData(data.AccountInfoId, false);
    //    }
    //});
});

//show or hide role checkboxx
function showhideRole(isAdminShow = true) {
    if (isAdminShow) {
        $('#admin').show();
        $('#trasaction').hide();
    } else {
        $('#admin').hide();
        $('#trasaction').show();
    }
}

//show or hide menu on menu table
function showMenu() {
    switch ($('input[name="IsAdmin"]:checked').val()) {
        case 'Transaction':
            if (menutable && $('#IsMaker').is(":checked")) {
                menutable.column(3).visible(true);
            }
            if (menutable && $('#IsViewer').is(":checked")) {
                menutable.column(4).visible(true);
            }
            if (menutable && $('#IsApprover').is(":checked")) {
                menutable.column(5).visible(true);
            }
            break;
        default:
            if (menutable && $('#IsAdminMaker').is(":checked")) {
                menutable.column(1).visible(true);
            }
            if (menutable && $('#IsAdminApprover').is(":checked")) {
                menutable.column(2).visible(true);
            }
    }
}

//show or hide Adminmaker menu on menu table
function showhideAdminMakerMenu(isShow) {
    if (isShow) {
        if (menutable) {
            menutable.column(1).visible(true);
        }
    } else {
        if (menutable) {
            menutable.column(1).visible(false);
        }
    }
}

//show or hide Adminapprover menu on menu table
function showhideAdminApproverMenu(isShow) {
    if (isShow) {
        if (menutable) {
            menutable.column(2).visible(true);
        }
    } else {
        if (menutable) {
            menutable.column(2).visible(false);
        }
    }
}

//show or hide maker menu on menu table
function showhideMakerMenu(isShow) {
    if (isShow) {
        if (menutable) {
            menutable.column(3).visible(true);
        }
        $('.Maker').show();
    } else {
        if (menutable) {
            menutable.column(3).visible(false);
        }
        $('.Maker').hide();
    }
}

//show or hide checker menu on menu table
function showhideCheckerMenu(isShow) {
    if (isShow) {
        if (menutable) {
            menutable.column(4).visible(true);
        }
    } else {
        if (menutable) {
            menutable.column(4).visible(false);
        }
    }
}

//show or hide approver menu on menu table
function showhideApproverMenu(isShow) {
    if (isShow) {
        if (menutable) {
            menutable.column(5).visible(true);
        }
        $('.Checker').show();
    } else {
        if (menutable) {
            menutable.column(5).visible(false);
        }
        $('.Checker').hide();
    }
}

//show or hide transaction info
function showhideTransInfo() {
    if ($('#IsMaker').is(":checked") || $('#IsViewer').is(":checked")) {
        $('[href="#step4"]').closest('li').show();
    } else {
        $('[href="#step4"]').closest('li').hide();
    }
}

//update Adminmaker menu data
function updateAdminMakerMenuData(menuId, status) {
    for (var i = 0; i < MenuInfo.length; i++) {
        if (MenuInfo[i].MenuInfoId == menuId) {
            MenuInfo[i].IsAdminMaker = status;
            break;
        }
    }
}

//update Adminapprover menu data
function updateAdminApproverMenuData(menuId, status) {
    for (var i = 0; i < MenuInfo.length; i++) {
        if (MenuInfo[i].MenuInfoId == menuId) {
            MenuInfo[i].IsAdminApprover = status;
            break;
        }
    }
}

//update maker menu data
function updateMakerMenuData(menuId, status) {
    for (var i = 0; i < MenuInfo.length; i++) {
        if (MenuInfo[i].MenuInfoId == menuId) {
            MenuInfo[i].IsMaker = status;
            break;
        }
    }
}

//update checker menu data
function updateCheckerMenuData(menuId, status) {
    for (var i = 0; i < MenuInfo.length; i++) {
        if (MenuInfo[i].MenuInfoId == menuId) {
            MenuInfo[i].IsViewer = status;
            break;
        }
    }
}

//update approver menu data
function updateApproverMenuData(menuId, status) {
    for (var i = 0; i < MenuInfo.length; i++) {
        if (MenuInfo[i].MenuInfoId == menuId) {
            MenuInfo[i].IsApprover = status;
            break;
        }
    }
}

//update account data
function updateAccData(accId, status) {
    for (var i = 0; i < AccInfo.length; i++) {
        if (AccInfo[i].AccountNo == accId) {
            AccInfo[i].IsActive = status;
            break;
        }
    }
}

//update to check/uncheck all account data
function toggleSelectAccData(status) {
    for (var i = 0; i < AccInfo.length; i++) {
        AccInfo[i].IsActive = status;
    }
}

//update acc title checkbox status
function checkAccDataStatus() {
    var isIndeterminate = false;
    var isTrue = false;
    var isFalse = false;
    for (var i = 0; i < AccInfo.length; i++) {
        if (AccInfo[i].IsActive == true && isFalse == false) {
            isTrue = true;
        } else if (AccInfo[i].IsActive == false && isTrue == false) {
            isFalse = true;
        } else {
            isIndeterminate = true;
            break;
        }
    }
    if (isIndeterminate) {
        $("#accTitleChk").prop({
            indeterminate: true,
            checked: false
        });
    } else if (isTrue) {
        $("#accTitleChk").prop({
            indeterminate: false,
            checked: true
        });
    } else {
        $("#accTitleChk").prop({
            indeterminate: false,
            checked: false
        });
    }
}

//validate registration form
var validator = $("#UserReg").validate({
    onkeyup: false, //turn off auto validate whilst typing
    rules: {
        CorporateUserId: { required: true, CheckCorporateUser: true },
        UserName: {
            required: true,
            spaceandspecialchar: true,
            minmaxlength: true,
            staringwithnumber: true,
            //CheckUserName: {
            //    depends: function () {
            //        return !$('#UserName').prop('disabled') && $("#UserName").val() != '';
            //    }
            //},
            //CheckReqUserName: {
            //    depends: function () {
            //        return !$('#UserName').prop('disabled') && $("#UserName").val() != '';
            //    }
            //}
            remote: {
                param: {
                    url: "/CorporateUserRequest/CheckValidUserName",
                    type: "post",
                    data: {
                        checkData: function () {
                            return $("#UserName").val();
                        },
                        checkDataID: function () {
                            return $("#CopRegId").val();
                        },
                        corporateId: function () {
                            return $("#CorpID").val();
                        }
                    },
                    error:
                        function(xhr, ajaxError, thrown) {
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
                    //    showAlertWithReload(exceptionThrown, errorMessages.join("<br />"));
                    //}
                },
                depends: function () {
                    return !$('#UserName').prop('disabled') && $("#UserName").val() != '' && $("#CorpID").val() != '';
                }
            }
        },
        FullName: {
            required: true,
            minlength: 3,
            maxlength: 50
        },
        Email: {
            required: true,
            regex: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
        },
        PhoneNo: {
            required: true,
            //minlength: 9,
            //maxlength: 13,
            staringwith09: true
        },
        Address: {
            required: true,
            minlength: 3,
            maxlength: 200
        },
        DepartmentId: { required: true },
        PositionId: { required: true },
        MakerTranLimit: {
            required: true,
            CheckTranLimit: true,
            min: 0,
            max: 9999999999999999
        },
        CheckerTranLimit: {
            required: true,
            CheckTranLimit: true,
            min: 0,
            max: 9999999999999999
        },
        MakerBulkTranLimit: {
            required: true,
            CheckTranLimit: true,
            min: 0,
            max: 9999999999999999
        },
        CheckerBulkTranLimit: {
            required: true,
            CheckTranLimit: true,
            min: 0,
            max: 9999999999999999
        }
        //IsAdmin: { CheckedAdmin: "#IsAdmin" }
    },
    messages: {
        CorporateUserId: { required: "The Corporate User field is required!" },
        UserName: {
            required: "The Username field is required!"
            //remote: jQuery.validator.format("{0} is already in use")
        },
        FullName: {
            required: "The Full Name field is required!",
            minlength: jQuery.validator.format("The field Full Name must be a string with a minimum length of {0}."),
            maxlength: jQuery.validator.format("The field Full Name must be a string with a maximum length of {0}.")
        },
        Email: {
            required: "The Email field is required!",
            regex: "The Email field is not a valid e-mail address."
        },
        PhoneNo: {
            required: "The Phone No field is required!",
            minlength: jQuery.validator.format("The field Phone No must be a string with a minimum length of {0}."),
            maxlength: jQuery.validator.format("The field Phone No must be a string with a maximum length of {0}.")
        },
        Address: {
            required: "The Address field is required!",
            minlength: jQuery.validator.format("The field Address must be a string with a minimum length of {0}."),
            maxlength: jQuery.validator.format("The field Address must be a string with a maximum length of {0}.")
        },
        DepartmentId: { required: "The Department field is required!" },
        PositionId: { required: "The Position field is required!" },
        MakerTranLimit: {
            required: "The Maker Tran Limit field is required!",
            min: jQuery.validator.format("Enter a value greater than or equal to {0}."),
            max: jQuery.validator.format("Enter a value less than or equal to {0}.")
        },
        CheckerTranLimit: {
            required: "The Checker Tran Limit field is required!",
            min: jQuery.validator.format("Enter a value greater than or equal to {0}."),
            max: jQuery.validator.format("Enter a value less than or equal to {0}.")
        },
        MakerBulkTranLimit: {
            required: "The Maker Bulk Tran Limit field is required!",
            min: jQuery.validator.format("Enter a value greater than or equal to {0}."),
            max: jQuery.validator.format("Enter a value less than or equal to {0}.")
        },
        CheckerBulkTranLimit: {
            required: "The Checker Bulk Tran Limit field is required!",
            min: jQuery.validator.format("Enter a value greater than or equal to {0}."),
            max: jQuery.validator.format("Enter a value less than or equal to {0}.")
        }
    },
    errorElement: "span",
    errorClass: "help-block",
    errorPlacement: function (b, c) {
        b.addClass("invalid-feedback text-danger-important"),
            "checkbox" === c.prop("type") ? b.insertAfter(c.parent("label")) :
                "radio" === c.prop("type") ? b.insertAfter(c.parent("label").parent("label")) :
                    b.insertAfter(c)
    },
    //highlight: function (element) {
    //    $(element).closest('.form-group').addClass('has-error');
    //},
    //unhighlight: function (element) {
    //    $(element).closest('.form-group').removeClass('has-error');
    //},
    highlight: function (element) { $(element).addClass('is-invalid'); },
    unhighlight: function (element) { $(element).removeClass('is-invalid'); },
    submitHandler: function (form, event) {
        event.preventDefault();
    }
});

//menu check
jQuery.validator.addMethod("CheckedAdmin",
    function (value, element, param) {
        if (!$(param).is(":checked")) {
            return $('#IsMaker').is(":checked") || $('#IsViewer').is(":checked") || $('#IsApprover').is(":checked");
        } else {
            return $('#IsMaker').is(":checked") || $('#IsApprover').is(":checked");
        }
    }, 'Need to check one role.');

//corporate user check
jQuery.validator.addMethod("CheckCorporateUser",
    function (value, element, param) {
        if ($('#CorpID').val() && $('#CorporateID').val() && $('#CIFID').val()) {
            return true;
        }
        return false;
    }, 'Invalid Company Name!');

//user name check
jQuery.validator.addMethod('spaceandspecialchar', function (value, element, param) {
    return /^[a-zA-Z0-9]+?$/.test(value);
}, 'Username is not allowed space and special character.');

//user name check
jQuery.validator.addMethod('minmaxlength', function (value, element, param) {
    return /^([a-zA-Z0-9]){6,16}$/.test(value);
}, 'Minimum length is 6 and maximum length is 16.');

//user name check
jQuery.validator.addMethod('staringwithnumber', function (value, element, param) {
    return /^[a-zA-Z]+([a-zA-Z0-9]){5,15}$/.test(value);
}, 'Username is not allowed to start with number.');

//ph no starting with 09, length 9-11
jQuery.validator.addMethod('staringwith09', function (value, element, param) {
    return /^[0][9]+\d{7,9}$/.test(value);
}, 'Phone no must start with 09. Minimum length is 9 and Maximum length is 11.');

//user name check remote
jQuery.validator.addMethod("CheckUserName", function (value, element, param) {
    if (!param) {
        return true;
    }
    var result;
    $.ajax({
        url: '/CorporateUserRequest/CheckUserName',
        type: "POST",
        data: {
            checkData: value,
            checkDataID: $("#CopRegId").val()
        },
        async: false,
        dataType: "json"
    }).done(function (response) {
        result = response;
    }).fail(
        function(xhr, ajaxError, thrown) {
            if (xhr.status == 302) {
                window.location.href = "/Admin/Login";
            }
        //function(xhr, textStatus, exceptionThrown) {
        //var errorData = $.parseJSON(xhr.responseText);
        //var errorMessages = [];
        //for (var key in errorData) {
        //    errorMessages.push(errorData[key]);
        //}
        //showAlert(exceptionThrown, errorMessages.join("<br />"));
    });
    return result;
}, 'Duplicate USERNAME');

//user name check remote
jQuery.validator.addMethod("CheckReqUserName", function (value, element, param) {
    if (!param) {
        return true;
    }
    var result;
    $.ajax({
        url: '/CorporateUserRequest/CheckReqUserName',
        type: "POST",
        data: {
            checkData: value,
            checkDataID: $("#CopRegId").val()
        },
        async: false,
        dataType: "json"
    }).done(function (response) {
        result = response;
    }).fail(
        function(xhr, ajaxError, thrown) {
            if (xhr.status == 302) {
                window.location.href = "/Admin/Login";
            }
        //function(xhr, textStatus, exceptionThrown) {
        //var errorData = $.parseJSON(xhr.responseText);
        //var errorMessages = [];
        //for (var key in errorData) {
        //    errorMessages.push(errorData[key]);
        //}
        //showAlert(exceptionThrown, errorMessages.join("<br />"));
    });
    return result;
}, 'Duplicate Requested USERNAME');

//to check Tran Limit with Corporate Limit
jQuery.validator.addMethod("CheckTranLimit", function (value, element, param) {
    var currentValue = parseInt(value, 10);
    var cLimit = parseInt($("#CorporateLimit").val(), 10);
    return currentValue <= cLimit;
}, "Corporate User Transaction Limit Can\'t Exceed The Corporate Transaction Limit.");

$.validator.addMethod(
    /* The value you can use inside the email object in the validator. */
    "regex",

    /* The function that tests a given string against a given regEx. */
    function (value, element, regexp) {
        /* Check if the value is truthy (avoid null.constructor) & if it's not a RegEx. (Edited: regex --> regexp)*/

        if (regexp && regexp.constructor != RegExp) {
            /* Create a new regular expression using the regex argument. */
            regexp = new RegExp(regexp);
        }

        /* Check whether the argument is global and, if so set its last index to 0. */
        else if (regexp.global) regexp.lastIndex = 0;

        /* Return whether the element is optional or the result of the validation. */
        return this.optional(element) || regexp.test(value);
    }
);

//check one of menu check
function checkItemInMenu(type) {
    var isFound = false;
    if (type == 1) {
        for (var i = 0; i < MenuInfo.length; i++) {
            if (MenuInfo[i].IsAdminMaker == true) {
                isFound = true;
                break;
            }
        }
    } else if (type == 2) {
        for (var i = 0; i < MenuInfo.length; i++) {
            if (MenuInfo[i].IsAdminApprover == true) {
                isFound = true;
                break;
            }
        }
    } else if (type == 3) {
        for (var i = 0; i < MenuInfo.length; i++) {
            if (MenuInfo[i].IsMaker == true) {
                isFound = true;
                break;
            }
        }
    } else if (type == 4) {
        for (var i = 0; i < MenuInfo.length; i++) {
            if (MenuInfo[i].IsViewer == true) {
                isFound = true;
                break;
            }
        }
    } else if (type == 5) {
        for (var i = 0; i < MenuInfo.length; i++) {
            if (MenuInfo[i].IsApprover == true) {
                isFound = true;
                break;
            }
        }
    }
    return isFound;
}

//check role info
function checkRoleInfo() {
    var isValid = false;
    switch ($('input[name="IsAdmin"]:checked').val()) {
        case 'Transaction':
            isValid = $('#IsMaker').is(":checked") || $('#IsViewer').is(":checked") || $('#IsApprover').is(":checked");
            break;
        default:
            isValid = $('#IsAdminMaker').is(":checked") || $('#IsAdminApprover').is(":checked");
    }
    return isValid;
}

//check menu item
function checkMenu() {
    var isCheck = false;
    switch ($('input[name="IsAdmin"]:checked').val()) {
        case 'Transaction':
            if ($('#IsMaker').is(":checked")) {
                isCheck = checkItemInMenu(3);
            }
            if (!isCheck && $('#IsViewer').is(":checked")) {
                isCheck = checkItemInMenu(4);
            }
            if (!isCheck && $('#IsApprover').is(":checked")) {
                isCheck = checkItemInMenu(5);
            }
            break;
        default:
            if ($('#IsAdminMaker').is(":checked")) {
                isCheck = checkItemInMenu(1);
            }
            if (!isCheck && $('#IsAdminApprover').is(":checked")) {
                isCheck = checkItemInMenu(2);
            }
    }
    return isCheck;
}

//check account item
function checkItemInAcc() {
    var isFound = false;

    //skip validation for Admin user
    var UserTypeRadio = $('input[type=radio][name=IsAdmin]:checked').val();
    if (UserTypeRadio == 'Admin')
        return true;

    for (var i = 0; i < AccInfo.length; i++) {
        if (AccInfo[i].IsActive == true) {
            isFound = true;
            break;
        }
    }
    return isFound;
}

//save
function SaveToServer(model, $this, callback) {
    var form = $('#UserReg');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: "/CorporateUserRequest/SaveReqCorporateUserReg",
        type: "POST",
        data: {
            __RequestVerificationToken: token,
            model: model,
            menuInfo: MenuInfo,
            accountInfo: AccInfo
        },
        async: true,
        dataType: "json"
    }).done(function (response) {
        showAlert(response.title, response.message, reqUserListUrl);
    }).fail(
        function(xhr, ajaxError, thrown) {
            if (xhr.status == 302) {
                window.location.href = "/Admin/Login";
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
function UpdateToServer(url, id, model, successUrl, $this, callback) {
    var form = $('#UserReg');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: url,
        type: "POST",
        data: {
            __RequestVerificationToken: token,
            id: id,
            model: model,
            menuInfo: MenuInfo,
            accountInfo: AccInfo
        },
        async: true,
        dataType: "json"
    }).done(function (response) {
        showAlert(response.title, response.message, successUrl);
    }).fail(function (xhr, ajaxError, thrown) {
            if (xhr.status == 302) {
                window.location.href = "/Admin/Login";
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

//bind view data to model
function ViewToModel() {
    var MakerTranLimit = 0;
    var CheckerTranLimit = 0;
    var MakerBulkTranLimit = 0;
    var CheckerBulkTranLimit = 0;
    var IsAdmin = false;
    var IsTransaction = false;
    var IsAdminMaker = false;
    var IsAdminApprover = false;
    var IsMaker = false;
    var IsViewer = false;
    var IsApprover = false;

    switch ($('input[name="IsAdmin"]:checked').val()) {
        case 'Transaction':
            IsTransaction = true;
            IsMaker = $('#IsMaker').is(":checked");
            IsViewer = $('#IsViewer').is(":checked");
            IsApprover = $('#IsApprover').is(":checked");
            if (IsMaker) {
                MakerTranLimit = $('#MakerTranLimit').val();
                MakerBulkTranLimit = $('#MakerBulkTranLimit').val();
            }
            if (IsApprover) {
                CheckerTranLimit = $('#CheckerTranLimit').val();
                CheckerBulkTranLimit = $('#CheckerBulkTranLimit').val();
            }
            break;
        default:
            IsAdmin = true;
            IsAdminMaker = $('#IsAdminMaker').is(":checked");
            IsAdminApprover = $('#IsAdminApprover').is(":checked");
    }

    var data = {
        CorpID: $('#CorpID').val(),
        CorporateID: $('#CorporateID').val(),
        CIFID: $('#CIFID').val(),
        UserName: $('#UserName').val(),
        FullName: $('#FullName').val(),
        Email: $('#Email').val(),
        PhoneNo: $('#PhoneNo').val(),
        Address: $('#Address').val(),
        DepartmentId: $('#DepartmentId').val(),
        PositionId: $('#PositionId').val(),
        MakerTranLimit: MakerTranLimit,
        CheckerTranLimit: CheckerTranLimit,
        MakerBulkTranLimit: MakerBulkTranLimit,
        CheckerBulkTranLimit: CheckerBulkTranLimit,
        IsAdmin: IsAdmin,
        IsTransaction: IsTransaction,
        IsAdminMaker: IsAdminMaker,
        IsAdminApprover: IsAdminApprover,
        IsMaker: IsMaker,
        IsViewer: IsViewer,
        IsApprover: IsApprover
    };
    return data;
}

//bind model to view
function ModelToView(data) {
    //Main
    $('#CorporateUserId').val(data.CompanyName);
    $('#CorpID').val(data.CorpID);
    $('#CorporateID').val(data.CorporateID);
    $('#CIFID').val(data.CIFID);
    $('#UserName').val(data.UserName);
    $('#FullName').val(data.FullName);
    $('#Email').val(data.Email);
    $('#PhoneNo').val(data.PhoneNo);
    $('#Address').val(data.Address);
    $('#DepartmentId').val(data.DepartmentId);
    $('#PositionId').val(data.PositionId);

    //Transaction
    $('#MakerTranLimit').val(data.MakerTranLimit);
    $('#CheckerTranLimit').val(data.CheckerTranLimit);
    $('#MakerBulkTranLimit').val(data.MakerBulkTranLimit);
    $('#CheckerBulkTranLimit').val(data.CheckerBulkTranLimit);

    //Role
    if (data.IsAdmin) {
        $("input[name=IsAdmin][value='Admin']").prop("checked", true);
        //show or hide role menu
        showhideRole(true);

        if (data.IsAdminMaker) {
            $("#IsAdminMaker").prop("checked", true);
            showhideAdminMakerMenu(true);
        } else {
            $("#IsAdminMaker").prop("checked", false);
            showhideAdminMakerMenu(false);
        }
        if (data.IsAdminApprover) {
            $("#IsAdminApprover").prop("checked", true);
            showhideAdminApproverMenu(true);
        } else {
            $("#IsAdminApprover").prop("checked", false);
            showhideAdminApproverMenu(false);
        }
    } else if (data.IsTransaction) {
        $("input[name=IsAdmin][value='Transaction']").prop("checked", true);
        //show or hide role menu
        showhideRole(false);

        if (data.IsMaker) {
            $("#IsMaker").prop("checked", true);
            showhideMakerMenu(true);
        } else {
            $("#IsMaker").prop("checked", false);
            showhideMakerMenu(false);
        }
        if (data.IsViewer) {
            $("#IsViewer").prop("checked", true);
            showhideCheckerMenu(true);
        } else {
            $("#IsViewer").prop("checked", false);
            showhideCheckerMenu(false);
        }
        if (data.IsApprover) {
            $("#IsApprover").prop("checked", true);
            showhideApproverMenu(true);
        } else {
            $("#IsApprover").prop("checked", false);
            showhideApproverMenu(false);
        }
    }
}

//update status
function ChangeToUpdateStatus(isReqUser = false) {
    $('#Register').text('Update');

    //disable input
    $("#CorporateUserId").prop('disabled', true);
    if (isReqUser) {
        $('#Register').val('UpdateReq');
        //$("#UserName").focus();
    } else {
        $("#UserName").prop('disabled', true);
        $('#Register').val('Update');
    }
}

//detail status
function ChangeToDetailStatus() {
    $('#Register').text('Back To List');
    $('#Register').val('Detail');

    //disable input
    //main
    $("#CorporateUserId").prop('disabled', true);
    $("#UserName").prop('disabled', true);
    $("#FullName").prop('disabled', true);
    $("#Email").prop('disabled', true);
    $("#PhoneNo").prop('disabled', true);
    $("#Address").prop('disabled', true);
    $("#DepartmentId").prop('disabled', true);
    $("#PositionId").prop('disabled', true);

    //transaction
    $("#MakerTranLimit").prop('disabled', true);
    $("#CheckerTranLimit").prop('disabled', true);
    $("#MakerBulkTranLimit").prop('disabled', true);
    $("#CheckerBulkTranLimit").prop('disabled', true);

    //role
    $('input[name=IsAdmin]').prop("disabled", true);
    $("#IsAdminMaker").prop('disabled', true);
    $("#IsAdminApprover").prop('disabled', true);
    $("#IsMaker").prop('disabled', true);
    $("#IsViewer").prop('disabled', true);
    $("#IsApprover").prop('disabled', true);

    //menu and account
    //$('table input[type=checkbox]').prop('disabled', true);
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

//to get Corporate Limit by selected C ID
function getCorporateLimit(cid) {
    var CorpLimit;
    $.post(getCorporateLimitUrl,
        {
            CorporateID: cid
        },
        function (response) {
            //console.log(response.limit);
            CorpLimit = response.limit;
        }).fail( function (xhr, ajaxError, thrown) {
                if (xhr.status == 302) {
                    window.location.href = "/Admin/Login";
                }
            //function (xhr, textStatus, exceptionThrown) {
            //var errorData = $.parseJSON(xhr.responseText);
            //var errorMessages = [];
            //for (var key in errorData) {
            //    errorMessages.push(errorData[key]);
            //}
            //showAlert(exceptionThrown, errorMessages.join("<br />"));
        }).done(function () {
            $("#CorporateLimit").val(CorpLimit);
        });
}