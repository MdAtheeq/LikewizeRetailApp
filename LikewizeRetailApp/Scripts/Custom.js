
function Brand() {
    $(document).ready(function () {
        $("#Brand").change(function () {


            $.ajax({
            type: 'POST',
            url: '@Url.Action("GetBrand","ReceivingOperations")',
            data: { prodType: $("#ProductType").val() },
            success: function (data) {
                $("#Brand").empty();
                $.each(data, function (i, brand) {
                    $("#Brand").append('<option value="' + brand.Value + '" > ' + brand.Text + '</option>')
                })

            }
            })

        })

    })
}


function Model() {
    $(document).ready(function () {
        $("#Brand").change(function () {
            $.ajax({
                type: 'POST',
                url: '@Url.Action("GetModel", "ReceivingOperations")',
                data: { prodType: $("#ProductType").val(), brand: $("#Brand").val() },
                success: function (data) {
                    $("#Model").empty();
                    $.each(data, function (i, model) {
                        $("#Model").append('<option value="' + model.Value + '" > ' + model.Text + '</option>')
                    })
                }
            })
        })
    })
}


    function CheckEmail() {
        var email1 = $("#Email").val();
    console.log(email1);
    $.ajax({

        type: 'POST',
    url: '/Admin/ValidateEmail',
    data: '{email: "' + email1 + '" }',
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (Count) {
                var message = $("#spanmsg");

    if (Count == "Yes") {
        message.html("Email id is not available");
    message.css("color", "red");
                }
    else {
        message.html("Email id is available");
    message.css("color", "green");
                }
            }
        })
    }

    function PageValidation() {
        var email = document.getElementById("Email");
    var customername = document.getElementById("CustomerName");
    var phonenumber = document.getElementById("PhoneNumber");
    var customermessage = $("#spanmsg1");
    var message = $("#spanmsg");
    var phonemessage = $("#spanmsg3");
    if (email.value.trim() == "") {
        message.html("This field is required");
    message.css("color", "red");
        }
    if (customername.value.trim() == "") {
        customermessage.html("This field is required");
    customermessage.css("color", "red");
        }
    if (phonenumber.value.trim() == "") {
        phonemessage.html("This field is required");
    phonemessage.css("color", "red");
        }

    if (email.value.trim() == "" || customername.value.trim() == "" || phonenumber.value.trim() == "") {
            return false;
        }
    else {
            return true;
        }
    }
