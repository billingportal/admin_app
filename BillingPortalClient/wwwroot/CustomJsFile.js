  // Place this script in the head to ensure it runs after jQuery is loaded
 $(document).ready(function () {

    var isFetching = false; // Flag to track if a request is in progress
    var SuggestionData;

    $('#CustomerName').on('input', function () {
        var inputValue = $(this).val();

        // Capitalize the input
        var capitalizedInput = inputValue.toUpperCase();

        // Make sure there is some input before making the AJAX request
        if (capitalizedInput.length >= 3 && !isFetching) {
            fetchSuggestions(capitalizedInput, "");
        } else {
            // Clear the suggestions if the input is less than 2 characters
            $('#suggestions-container').empty();
        }
    });


    function fetchSuggestions(keyword, salesRegions) {
        console.log("fetchSuggestions called with keyword:", keyword, "and salesRegions:", salesRegions);
        // Set the flag to true before making the AJAX request
        isFetching = true;

        $.ajax({
            url: '/Customer/GetSuggestionsByKeyword',
            type: 'POST',
            dataType: 'json',
            data: { keyword: keyword, salesRegions: salesRegions },
            success: function (data) {
                console.log("AJAX success:", data);
                SuggestionData = data;
                displaySuggestions(SuggestionData);
            },
            error: function (error) {
                console.error('Error fetching suggestions:', error);
            },
            complete: function () {
                console.log("AJAX request complete");
                // Reset the flag to false after the request is complete
                isFetching = false;
            }
        });
    }

    // Define the suggestionsContainer outside the function scope
    var suggestionsContainer = $('#suggestions-container');
    var emailContainer = $('#email-container');
    var accountContainer = $('#account-container');

    function displaySuggestions(suggestions) {
        console.log("Inside displaySuggestions with suggestions:", suggestions);
        // Clear existing suggestions
        suggestionsContainer.empty();

        // Display suggestions in the dropdown
        $.each(suggestions, function (index, suggestion) {
            var suggestionText = suggestion.accountName;
            console.log("Displaying suggestion:", suggestionText);

            // Append the suggestion to the container
            suggestionsContainer.append('<div class="suggestion-item" data-suggestion-index="' + index + '">' + suggestionText + '</div>');
        });

        // Re-attach click event handler to suggestion items after they are added
        $('.suggestion-item').on('click', function () {
            var suggestionIndex = $(this).data('suggestion-index');
            var selectedSuggestion = suggestions[suggestionIndex];
            console.log("Suggestion item clicked:", selectedSuggestion);

            // Update CustomerName input
            $('#CustomerName').val(selectedSuggestion.accountName);
            emailContainer.empty();

            // Populate the customers dropdown with the selected suggestion email
            if (selectedSuggestion.email) {
                var $option = $('<div class="email-item" onclick="' + emailSelect(selectedSuggestion.email) +'">' + selectedSuggestion.email + '</div>');
                emailContainer.append($option);
            }

        });

    }
    
    function emailSelect(selectedEmail) {
        console.log("inside selected email");
       
        console.log("CustomerEmail dropdown changed, selected email:", selectedEmail);
    
        var selectedSuggestion = SuggestionData.find(s => s.email === selectedEmail);
        console.log("Found selected suggestion:", selectedSuggestion);
    
        if (selectedSuggestion && selectedSuggestion.accountNumber) {
            console.log("Selected suggestion for email:", selectedSuggestion);

            accountContainer.empty(); // Clear existing options
    
            var $option = $('<div class="account-item" onclick="' + accountSelect(selectedSuggestion.accountNumber) +'">' + selectedSuggestion.accountNumber + '</div>');
            accountContainer.append($option);
            
        } else {
            console.log("No matching suggestion found for the selected email.");
        }
    };

    function accountSelect(selectedAccount) {
        console.log("inside selected account");
       
        console.log("CustomerAccount dropdown changed, selected account:", selectedAccount);
    
        var selectedSuggestion = SuggestionData.find(s => s.accountNumber === selectedAccount);
        console.log("Found selected suggestion:", selectedSuggestion);
    
        if (selectedSuggestion) {
            console.log("Selected suggestion for account number:", selectedSuggestion);
            // Send the selected suggestion data to the server to update the user claims
            $.ajax({
                url: '/Authentication/updateCustomerClaims', // Your API endpoint to update user claims
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(selectedSuggestion),
                success: function (response) {
                    console.log('User claims updated successfully:', response);
                    window.location.href = '/Invoice/Index';
                },
                error: function (error) {
                    console.error('Error updating user claims:', error);
                }
            });
        } else {
            console.log("No matching suggestion found for the selected account number.");
        }
    };

     // Filter email items based on input in CustomerEmail
     $('#CustomerEmail').on('input', function() {
        var inputValue = $(this).val().toLowerCase();
        $('.email-item').each(function() {
            var email = $(this).text().toLowerCase();
            if (email.includes(inputValue)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });

     // Filter account items based on input in CustomerAccount
     $('#CustomerAccount').on('input', function() {
        var inputValue = $(this).val().toLowerCase();
        $('.account-item').each(function() {
            var account = $(this).text().toLowerCase();
            if (account.includes(inputValue)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
   

     $.ajax({
        url: "/Authentication/GetCurrentAdmin",
        type: 'GET',
        dataType: 'json', // added data type
        success: function (res) {
            console.log(res);

           //$('#changeAccountDropDown').append(optionsAsString);
            $("#adminNameText").text(res.firstName);
            $("#adminDesignationText").text(res.role);
            $("#adminEmailText").text(res.email)
            //var changeDrop = document.getElementById("changeAccountDropDown");
        }
    });

});



 // Ensure the CustomerEmail dropdown exists before attaching the event handler

 



// Ensure the CustomerAccount dropdown exists before attaching the event handler
if ($('#CustomerAccount').length) {
    $(document).on('change', '#CustomerAccount', function() {
        var selectedAccountNumber = $(this).val();
        console.log("CustomerAccount dropdown changed, selected account number:", selectedAccountNumber);

        var selectedSuggestion = SuggestionData.find(s => s.accountNumber === selectedAccountNumber);
        console.log("Found selected suggestion:", selectedSuggestion);

        if (selectedSuggestion) {
            console.log("Selected suggestion for account number:", selectedSuggestion);
            // Send the selected suggestion data to the server to update the user claims
            $.ajax({
                url: '/Authentication/updateCustomerClaims', // Your API endpoint to update user claims
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(selectedSuggestion),
                success: function (response) {
                    console.log('User claims updated successfully:', response);
                },
                error: function (error) {
                    console.error('Error updating user claims:', error);
                }
            });
        } else {
            console.log("No matching suggestion found for the selected account number.");
        }
    });
} else {
    console.error("CustomerAccount element not found.");
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