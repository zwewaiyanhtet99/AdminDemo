function changeValue(dropdown) {
    var option = dropdown.options[dropdown.selectedIndex].text;
    var obj = new Object;
    obj.usertype = option;
    if (option != null) {
       
        $('#rmname').val(option);
       
    }
  
}

function changeValueRM(dropdown) {
    var option = dropdown.options[dropdown.selectedIndex].value;
    var obj = new Object;
    obj.usertype = option;
    if (option != null) {

        $('#rmname').val(option);

    }
   
}