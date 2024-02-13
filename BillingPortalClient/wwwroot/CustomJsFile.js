  // Place this script in the head to ensure it runs after jQuery is loaded
  $(document).ready(function () {

     //alert('document is ready')
     $.ajax({
        url: "/Authentication/GetCurrentCustomer",
        type: 'GET',
        dataType: 'json', // added data type
        success: function (res) {
            console.log(res);

            var optionsAsString = "";

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


        }
    });

   // Add an event listener for region dropdown change
$('#Location').on('change', function () {
    var selectedRegion = $(this).val();

    // Call API to get customers by location
    $.ajax({
        url: '/CustomerSel/GetCustomersByLocation',
        type: 'POST',
        dataType: 'json',
        data: { location: selectedRegion }, // Pass the selected region as data
        success: function (customers) {
            // Assuming customers is an array of CustomerList objects
            console.log(customers);

            // Populate the customers dropdown
            var customersDropdown = $('#CustomerEmail');
            customersDropdown.empty(); // Clear existing options

            // Populate options
            $.each(customers, function (index, customer) {
                customersDropdown.append($('<option>', {
                    value: customer.email,
                    text: customer.email
                }));
            });
        },
        error: function (error) {
            console.error('Error fetching customers by location:', error);
        }
    });
});

// Initial population of the region dropdown
function populateRegionDropdown() {
    // Call API to get regions
    $.ajax({
        url: '/CustomerSel/GetAllCustomersRegion',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // Assuming data is an array of regions
            console.log(data);
            var regionDropdown = $('#Location');
            regionDropdown.empty(); // Clear existing options

            // Populate options
            $.each(data, function (index, region) {
                regionDropdown.append($('<option>', {
                    value: region,
                    text: region
                }));
            });
        },
        error: function (error) {
            console.error('Error fetching regions:', error);
        }
    });
}

// Populate the region dropdown
populateRegionDropdown();



    var emailInput = $('#email');

    emailInput.typeahead({
        source: function (query, process) {
            $.ajax({
                url: '/CustomerSel/GetEmailsByCustomer/', // Update the URL based on your API endpoint
                type: 'GET',
                data: { query: query },
                dataType: 'json',
                success: function (data) {
                    process(data);
                }
            });
        },
        afterSelect: function (item) {
            // Trigger the population of accounts after selecting an email
            populateAccounts(item.id); // Assuming 'id' is the property containing emailId
        }
    });

});


function populateAccounts(emailId) {
    $.ajax({
        url: '/CustomerSel/GetAccounts', // Get Accounts based on emails
        type: 'GET',
        data: { emailId: emailId },
        dataType: 'json',
        success: function (data) {
            // Assuming there's a function to update the accounts dropdown
            updateAccountsDropdown(data);
        }
    });
}

function updateAccountsDropdown(accounts) {
        console.log(accounts); // Log the data for debugging
}

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