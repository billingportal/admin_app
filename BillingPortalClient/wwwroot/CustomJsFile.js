  // Place this script in the head to ensure it runs after jQuery is loaded
  $(document).ready(function () {

    $('#CustomerName').on('input', function () {
        var inputValue = $(this).val();
    
        // Capitalize the input
        var capitalizedInput = inputValue.toUpperCase();
    
        // Make sure there is some input before making the AJAX request
        if (capitalizedInput.length >= 2) {
            fetchSuggestions(capitalizedInput);
        } else {
            // Clear the suggestions if the input is less than 2 characters
            $('#suggestions-container').empty();
        }
    });

    function fetchSuggestions(keyword) {
        $.ajax({
            url: '/CustomerSel/GetSuggestions',
            type: 'POST',
            dataType: 'json',
            data: { keyword: keyword },
            success: function (data) {
                displaySuggestions(data);
            },
            error: function (error) {
                console.error('Error fetching suggestions:', error);
            }
        });
    }

    // Define the suggestionsContainer outside the function scope
    var suggestionsContainer = $('#suggestions-container');

    function displaySuggestions(suggestions) {
        // Clear existing suggestions
        suggestionsContainer.empty();

        // Display suggestions in the dropdown
        $.each(suggestions, function (index, suggestion) {
            // Assuming 'accountName' is the property to display
            var suggestionText = suggestion.accountName;

            // Append the suggestion to the container
            suggestionsContainer.append('<div class="suggestion-item">' + suggestionText + '</div>');
        });

        // Handle click on a suggestion
        $('.suggestion-item').on('click', function () {
            var selectedSuggestion = $(this).text();
            $('#CustomerName').val(selectedSuggestion);
            suggestionsContainer.empty();

            // Call the API with the selected company
            $.ajax({
                url: '/CustomerSel/GetCustomersByCompany',
                type: 'POST',
                dataType: 'json',
                data: { company: selectedSuggestion }, // Pass the selected company as data
                success: function (customers) {
                    // Assuming customers is an array of CustomerList objects
                    // Populate the customers dropdown
                    var customersDropdown = $('#CustomerEmail');
                    customersDropdown.empty(); // Clear existing options

                    // Populate options
                    $.each(customers, function (index, customer) {
                        console.log(customer);

                        $option = $('<option class="selected-email" value="' + customer.email + '">' + customer.email + '</option>');
                    
                        $option.attr('selected', 'selected');
                    
                        customersDropdown.append($option);
                       
                    });
                    
                },
                error: function (error) {
                    console.error('Error fetching customers by Company:', error);
                }
            });
        });

        document.addEventListener('click', function(event) {
            if (event.target && event.target.id === 'CustomerEmail') {
                // Get the selected email
                var selectedEmail = event.target.value;
                console.log("selectedEmail", selectedEmail);
                // Call the function to populate accounts based on the selected email
                populateAccounts(selectedEmail);
            }
        });
      
       
    }



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
                    $option = $('<option id="selected-email" value="' + accounts[i].id + '">' + accounts[i].accountName + '</option>');
                    
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
            $("#customerNameText").text(res.name);
            $("#customerDesignationText").text(res.designation);
            $("#customerEmailText").text(res.email)

            //var changeDrop = document.getElementById("changeAccountDropDown");
        }
    });

    

});



function populateAccounts(emailId) {
    console.log("Populate account calling");
    $.ajax({
        url: '/CustomerSel/GetAccountsByEmail/', // Get Accounts based on emails
        type: 'POST',
        data: { emailId: emailId },
        dataType: 'json',
        success: function (data) {
            // Assuming there's a function to update the accounts dropdown
            console.log("accounts", data);
            updateAccountsDropdown(data);
        }
    });
}

function updateAccountsDropdown(accounts) {
    $('#CustomerAccount').empty();
          
    // Iterate over the accounts and add them as options
    $.each(accounts, function(index, account) {
        $('#CustomerAccount').append('<option value="' + account.accountNumber + '">' + account.accountNumber + '</option>');
    });
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