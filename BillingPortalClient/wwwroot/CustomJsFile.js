

$(document).ready(function () {


    //alert('document is ready')
    $.ajax({
        url: "/Authentication/GetCurrentCustomer",
        type: 'GET',
        dataType: 'json', // added data type
        success: function (res) {
            console.log(res);

            var optionsAsString = "";
            //for (var i = 0; i < res.length; i++) {
            //    string selected = "";
            //    if (res[i].isSelected == true) {
            //        selected = "selected";
            //    }
            //    optionsAsString += "<option value='" + res[i].id + "' "+selected+">" + res[i].accountName + "</option>";
            //}

            let accounts = [];

            if (res.isMain == false) {
                accounts.push(res.customerUserCustomers[0].parentAccount);
                for (var i = 0; i < accounts.length; i++) {
                    $option = $('<option value="' + accounts[i].id + '">' + accounts[i].accountName + '</option>');
                    
                    $option.attr('selected', 'selected');
                    
                    $('#changeAccountDropDown').append($option);
                }
            }
            else {
                accounts = res.accounts
                for (var i = 0; i < accounts.length; i++) {
                    $option = $('<option value="' + accounts[i].id + '">' + accounts[i].accountName + '</option>');
                    if (accounts[i].isSelected == true) {
                        $option.attr('selected', 'selected');
                    }
                    $('#changeAccountDropDown').append($option);
                }
            }
            //$('#changeAccountDropDown').append(optionsAsString);

            //var changeDrop = document.getElementById("changeAccountDropDown");
        }
    });

    $.ajax({
        url: "/Authentication/GetCurrentCustomer",
        type: 'GET',
        dataType: 'json', // added data type
        success: function (res) {
            console.log(res);

            $("#customerNameText").text(res.name);
            $("#customerDesignationText").text(res.designation);
            $("#customerEmailText").text(res.email)

            //var optionsAsString = "";


            //for (var i = 0; i < res.length; i++) {
            //    $option = $('<option value="' + res[i].id + '">' + res[i].accountName + '</option>');
            //    if (res[i].isSelected == true) {
            //        $option.attr('selected', 'selected');
            //    }
            //    $('#changeAccountDropDown').append($option);
            //}

        }
    });
});


function changeCustomerAccount(e) {
    //alert(window.location)
    var account = new Object();
    //employee.id = 2;

    var changeDrop = document.getElementById("changeAccountDropDown");
    var value = changeDrop.value;
    var text = changeDrop.options[changeDrop.selectedIndex].text;
    //alert(value, text)
    account.accountId = value;
    account.currentUrl = window.location.pathname;

    if (account != null) {
        //console.log({ employee })
        $.ajax({
            type: "POST",
            url: "/authentication/ChangeCustomerAccount",
            data: JSON.stringify(account),
            contentType: "application/json; charset=utf-8",
            //dataType: "json",
            success: function (response) {
                //alert('asdfasf')
                window.location.href = response;
                //if (response != null) {  
                //    alert("Name : " + response.Name + ", Designation : " + response.Designation + ", Location :" + response.Location);  
                //} else {  
                //    alert("Something went wrong");  
                //}  
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
}