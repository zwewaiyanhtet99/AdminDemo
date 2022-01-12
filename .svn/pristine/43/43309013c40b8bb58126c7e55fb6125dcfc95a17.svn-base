function changeValue(dropdown) {
    var option = dropdown.options[dropdown.selectedIndex].text;
    var obj = new Object;
    obj.usertype = option;
    if (option != null) {

        $('#usertype').val(option);
    }
    if (obj.usertype == "" || obj.usertype == undefined) {
        $("#pMessage").text("UserType is required.");
        $("#divError").modal('toggle');
        return;
    }
    $("#booking").val("");
    $(".field-validation-error").empty();
    //$('#booking').removeClass().addClass('field-validation-error');
    $.ajax({
        type: "GET",
        url: "/Number_Management/GetUserTypeLimit",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        data: obj,
        cache: false,
        success: function (response) {
            
            if (response.USERTYPE_CODE_LIMIT != null) {
                $("#prefixbooking").val(response.USERTYPE_CODE_LIMIT); // before reducing the maxlength, make sure it contains at most two characters; you could also reset the value altogether
                var prefix = $("#prefixbooking").val().replace('-', '');
                var maxlengthval = response.GENERATED_LIMIT - prefix.length;
                $("#booking").prop('maxlength', maxlengthval);
                $("#booking").attr('readonly', false);
               

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


function userPermission() {
    $("#errorMessage").text("You don't have permission !");
    $("#userPermissionError").modal('toggle');
}

