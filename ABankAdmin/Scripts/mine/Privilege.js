function changeValuePrivilege(dropdown) {
    var option = dropdown.options[dropdown.selectedIndex].value;
    var obj = new Object;
    obj.privilegeID = option;
   
    if (option != null) {

        $('#name').val(option);
       
        if (obj.privilegeID != null) {
         $.ajax({
            type: "GET",
            url: "/PrivilegeUsage/getPrivilegeInfo",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            data: obj,
            cache: false,
            success: function (data) {
                if (data.DateAndTime != null && data.Time != null) {
                    $("#privilegedate").val(data.DateAndTime);
                    $("#privilegetime").val(data.Time);
                    $("#location").val(data.Location);
                    $("#followers").val(data.NoOfFollowers);
                    $("#staff").val(data.Staff);
              
                return;
        }
        

        }
        });
        }
        //$('#privilegedate').val(option);
        //$('#name').val(option);
        //$('#name').val(option);
        //$('#name').val(option);
        //$('#name').val(option);

    }

}

function checkInfo() {
    var obj = getInputData();
    //to check iconic number is null
    if (obj.IconicNo == "" || obj.IconicNo == undefined) {
        $("#pMessage").text("Iconic is required.");
        $("#divError").modal('toggle');
        return;
    }


    //clear existing searched data
    $("#IconicTier").val('');
    $("#IconicNumber").val('');
    $("#IconicExpireDate").val('');
    $("#accTbody > tr").remove();
    //$("#awttbody > tr").remove();
    //get new by CIFID
    $.ajax({
        type: "GET",
        url: "/PrivilegeUsage/getCIFIDInfo",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        data: obj,
        cache: false,
        success: function (data) {
            if (data.IconicNumber != null && data.IconicTier != null) {
                $("#IconicTier").val(data.IconicTier);
                $("#IconicNumber").val(data.IconicNumber);
                $("#IconicExpireDate").val(data.IconicExpireDate);
                $("#NRC").val(data.NRC);
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

        }
    });
    return;
}
function getInputData() {
    return { IconicNo: $("#IconicNumber").val().trim() };
}
function userPermission() {
    $("#errorMessage").text("You don't have permission !");
    $("#userPermissionError").modal('toggle');
}