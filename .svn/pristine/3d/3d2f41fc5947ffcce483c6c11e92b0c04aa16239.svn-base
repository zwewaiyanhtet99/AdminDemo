function check() {
    var obj = getInputData();
    //to check CIFID is null
    if (obj.CIFID == "" || obj.CIFID == undefined) {
        return;
    }
    //clear existing searched data
    $("#uservm_CIFID").val('');
    $("#uservm_MOBILENO").val('');
    $("#uservm_FULLNAME").val('');
    $("#uservm_NRC").val('');
    $("#uservm_EMAIL").val('');
    $("#uservm_ADDRESS").val('')
    //$("#uservm_MINOR").val('');
    //$("#uservm_GENDER").val('');
    $("#uservm_USERNAME").val('');
    $("#accTbody > tr").remove();
    //$("#awttbody > tr").remove();
    //get new by CIFID
    $.ajax({
        type: "GET",
        url: "/User/getCorporate",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        data: obj,
        cache: false,
        success: function (data) {
            if (data.Name != null) {
                $("#uservm_CIFID").val(obj.CIFID);
                $("#uservm_MOBILENO").val(data.PhoneNumber);
                $("#uservm_FULLNAME").val(data.Name);
                $("#uservm_NRC").val(data.AccountOpenDate);
                $("#uservm_EMAIL").val(data.Email);
                //$("#uservm_MINOR").val(data.Minor);
                //$("#uservm_GENDER").val(data.Gender);

                //loop all accounts
                if (data.lstCorpAccountInfo != undefined && data.lstCorpAccountInfo != null) {
                    //acc table
                    var accTable = document.getElementsByTagName('tbody')[0];
                    for (var i = 0; i < data.lstCorpAccountInfo.length; i++) {
                        var tr = accTable.insertRow();      // TABLE ROW.
                        //tr.setAttribute('id', 'acc_tr' + i);

                        //Row No
                        var td = document.createElement('td');          // TABLE DEFINITION.
                        td = tr.insertCell(0);
                        var rowno = document.createTextNode(i + 1);
                        td.setAttribute('style', "width:50px;text-align:center;");
                        td.appendChild(rowno);

                        //AccNo column
                        var td = document.createElement('td');
                        td = tr.insertCell(1);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:165px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["AccountNumber"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].ACCOUNTNO');
                        ele.setAttribute('id', 'accountvms_' + i + '__ACCOUNTNO');
                        td.appendChild(ele);

                        //Acc Type column
                        var td = document.createElement('td');
                        td = tr.insertCell(2);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:160px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["AccountType"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].ACC_TYPE');
                        ele.setAttribute('id', 'accountvms_' + i + '__ACC_TYPE');
                        td.appendChild(ele);

                        //Acc Scheme Code
                        var td = document.createElement('td');
                        td = tr.insertCell(3);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:160px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["Schm_Code"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].SCHM_CODE');
                        ele.setAttribute('id', 'accountvms_' + i + 'SCHM_CODE');
                        td.appendChild(ele);

                        //Branch column
                        var td = document.createElement('td');
                        td = tr.insertCell(4);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:180px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["BranchID"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].BRANCHCODE');
                        ele.setAttribute('id', 'accountvms_' + i + '__BRANCHCODE');
                        td.appendChild(ele);

                        //Currency column
                        var td = document.createElement('td');
                        td = tr.insertCell(5);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:120px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["Currency"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].CURRENCY');
                        ele.setAttribute('id', 'accountvms_' + i + '__CURRENCY');
                        td.appendChild(ele);

                        //checkbox column
                        var td = document.createElement('td');          // TABLE DEFINITION.
                        td = tr.insertCell(6);
                        var cbox = document.createElement('input');
                        cbox.setAttribute('type', 'checkbox');// SET INPUT ATTRIBUTE.
                        cbox.setAttribute('value', true);
                        td.setAttribute('style', "width:80px;text-align:center;");
                        cbox.setAttribute('name', 'accountvms[' + i + '].Active');
                        cbox.setAttribute('id', 'accountvms_' + i + '__Active');
                        //check change event
                        cbox.setAttribute('onclick', 'checkChange(' + i + ')');
                        td.appendChild(cbox);

                        //QR Allow
                        var td = document.createElement('td');
                        td = tr.insertCell(7);
                        var cbox = document.createElement('input');
                        cbox.setAttribute('type', 'checkbox');// SET INPUT ATTRIBUTE.
                        cbox.setAttribute('value', true);
                        td.setAttribute('style', "width:90px;text-align:center;");
                        
                        cbox.setAttribute('disabled', 'disabled');
                        cbox.setAttribute('name', 'accountvms[' + i + '].QR_ALLOW');
                        cbox.setAttribute('id', 'accountvms_' + i + '__QR_ALLOW');
                        td.appendChild(cbox);

                        //Acc Type column
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'hidden');
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["AccountTypeDesc"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].ACC_DESC');
                        ele.setAttribute('id', 'accountvms_' + i + '__ACC_DESC');
                        td.appendChild(ele);
                    }
                }

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
            if (status != null) {

                userPermission();
            }
            console.log(xhr.responseText);
        }
    });
    return;
}

function getInputData() {
    return { CIFID: $("#uservm_CIFID").val().trim() };
}

//Checking by Request controller for maker
function CheckFromRequest() {
    var obj = getInputData();
    //to check CIFID is null
    if (obj.CIFID == "" || obj.CIFID == undefined) {
        return;
    }
    //clear existing searched data
    $("#uservm_CIFID").val('');
    $("#uservm_MOBILENO").val('');
    $("#uservm_FULLNAME").val('');
    $("#uservm_NRC").val('');
    $("#uservm_EMAIL").val('');
    $("#uservm_ADDRESS").val('')
    //$("#uservm_MINOR").val('');
    //$("#uservm_GENDER").val('');
    $("#uservm_USERNAME").val('');
    $("#accTbody > tr").remove();
    //$("#awttbody > tr").remove();
    //get new by CIFID
    $.ajax({
        type: "GET",
        url: "/Request/getCorporate",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        data: obj,
        cache: false,
        success: function (data) {
            if (data.Name != null) {
                $("#uservm_CIFID").val(obj.CIFID);
                $("#uservm_MOBILENO").val(data.PhoneNumber);
                $("#uservm_FULLNAME").val(data.Name);
                $("#uservm_NRC").val(data.AccountOpenDate);
                $("#uservm_EMAIL").val(data.Email);
                //$("#uservm_MINOR").val(data.Minor);
                //$("#uservm_GENDER").val(data.Gender);

                //loop all accounts
                if (data.lstCorpAccountInfo != undefined && data.lstCorpAccountInfo != null) {
                    //acc table
                    var accTable = document.getElementsByTagName('tbody')[0];
                    for (var i = 0; i < data.lstCorpAccountInfo.length; i++) {
                        var tr = accTable.insertRow();      // TABLE ROW.
                        //tr.setAttribute('id', 'acc_tr' + i);

                        //Row No
                        var td = document.createElement('td');          // TABLE DEFINITION.
                        td = tr.insertCell(0);
                        var rowno = document.createTextNode(i + 1);
                        td.setAttribute('style', "width:50px;text-align:center;");
                        td.appendChild(rowno);

                        //AccNo column
                        var td = document.createElement('td');
                        td = tr.insertCell(1);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:165px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["AccountNumber"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].ACCOUNTNO');
                        ele.setAttribute('id', 'accountvms_' + i + '__ACCOUNTNO');
                        td.appendChild(ele);

                        //Acc Type column
                        var td = document.createElement('td');
                        td = tr.insertCell(2);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:160px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["AccountType"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].ACC_TYPE');
                        ele.setAttribute('id', 'accountvms_' + i + '__ACC_TYPE');
                        td.appendChild(ele);

                        //Acc Scheme Code
                        var td = document.createElement('td');
                        td = tr.insertCell(3);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:160px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["Schm_Code"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].SCHM_CODE');
                        ele.setAttribute('id', 'accountvms_' + i + 'SCHM_CODE');
                        td.appendChild(ele);

                        //Branch column
                        var td = document.createElement('td');
                        td = tr.insertCell(4);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:180px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["BranchID"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].BRANCHCODE');
                        ele.setAttribute('id', 'accountvms_' + i + '__BRANCHCODE');
                        td.appendChild(ele);

                        //Currency column
                        var td = document.createElement('td');
                        td = tr.insertCell(5);
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'text');// SET INPUT ATTRIBUTE.
                        ele.setAttribute('readonly', 'readonly');
                        ele.setAttribute('class', 'form-control');
                        td.setAttribute('style', "width:120px");
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["Currency"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].CURRENCY');
                        ele.setAttribute('id', 'accountvms_' + i + '__CURRENCY');
                        td.appendChild(ele);

                        //checkbox column
                        var td = document.createElement('td');          // TABLE DEFINITION.
                        td = tr.insertCell(6);
                        var cbox = document.createElement('input');
                        cbox.setAttribute('type', 'checkbox');// SET INPUT ATTRIBUTE.
                        cbox.setAttribute('value', true);
                        td.setAttribute('style', "width:80px;text-align:center;");
                        cbox.setAttribute('name', 'accountvms[' + i + '].Active');
                        cbox.setAttribute('id', 'accountvms_' + i + '__Active');
                        //check change event
                        cbox.setAttribute('onclick', 'checkChange(' + i + ')');
                        td.appendChild(cbox);

                        //QR Allow
                        var td = document.createElement('td');
                        td = tr.insertCell(7);
                        var cbox = document.createElement('input');
                        cbox.setAttribute('type', 'checkbox');// SET INPUT ATTRIBUTE.
                        cbox.setAttribute('value', true);
                        td.setAttribute('style', "width:90px;text-align:center;");

                        cbox.setAttribute('disabled', 'disabled');
                        cbox.setAttribute('name', 'accountvms[' + i + '].QR_ALLOW');
                        cbox.setAttribute('id', 'accountvms_' + i + '__QR_ALLOW');
                        td.appendChild(cbox);

                        //Acc Type column
                        var ele = document.createElement('input');
                        ele.setAttribute('type', 'hidden');
                        ele.setAttribute('value', data.lstCorpAccountInfo[i]["AccountTypeDesc"]);
                        ele.setAttribute('name', 'accountvms[' + i + '].ACC_DESC');
                        ele.setAttribute('id', 'accountvms_' + i + '__ACC_DESC');
                        td.appendChild(ele);
                    }
                }

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
            if (status != null) {

                userPermission();
            }
            console.log(xhr.responseText);
        }
    });
    return;
}

//removing accouts if form cancel
function cancel() {
    var form = document.getElementsByTagName('form')[0];
    form.reset();
    $("#accTbody > tr").remove();
}

//allow check change
function checkChange(row) {
    var cbox = $('#accountvms_' + row + '__Active')[0];
    var acctype = $('#accountvms_' + row + '__ACC_TYPE')[0];
    var acccurrency = $('#accountvms_' + row + '__CURRENCY')[0];
    acctype = acctype.value;
    acccurrency = acccurrency.value;
    var isAllowed = (acctype != 'ODA' && acctype != 'LAA' && acctype !== 'TDA');
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