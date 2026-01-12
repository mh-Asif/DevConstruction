$(document).ready(function () {
    //window.session.monitorAuthenticationTimeout(
    //    "/Account/Login",    // You could also use "@FormsAuthentication.LoginUrl" in Razor.
    //    "/Account/Ping");

  //  $("select").addClass("select2");
   // $(".select2").select2();
    //dateDiffrenece();
    //BlockUI();
   // ActiveCurrentPageLink();
    //BlockUI();

    // fetchNotifications();
   
});

//(function ($, undefined) {

//    if (!window.session) {

//        window.session = {

//            monitorAuthenticationTimeout: function (redirectUrl, pingUrl, warningDuration, cushion) {

//                // If params not specified, use defaults.
//                redirectUrl = redirectUrl || "~/Account/Login";
//                pingUrl = pingUrl || "~/Account/Ping";
//                warningDuration = warningDuration || 60;
//                cushion = cushion || 4000;

//                var timeoutStartTime = 0,
//                    timeout,
//                    timer,
//                    popup,
//                    countdown,
//                    pinging;

//                var updateCountDown = function () {
//                    //var secondsRemaining = Math.floor((timeout - ((new Date()).getTime() - timeoutStartTime)) / 1000),
//                    //    min = Math.floor(secondsRemaining / 60),
//                    //    sec = secondsRemaining % 60;
//                    var secondsRemaining = timeoutStartTime - 1;
//                    // If timeout hasn't expired, continue countdown.
//                    if (secondsRemaining >= 0) {
//                        countdown.text((secondsRemaining < 10 ? "0" + secondsRemaining : secondsRemaining));
//                        timer = window.setTimeout(updateCountDown, 1000);
//                        timeoutStartTime = secondsRemaining;

//                    }
//                    // Else redirect to login.
//                    else if (secondsRemaining + 2 <= 0) {
//                        window.location = redirectUrl;
//                    }
//                };

//                var showWarning = function () {
//                    if (!popup) {
//                        // countdown = serverTimeOutInSeconds - 1;
//                        popup = $(
//                            "<div style=\"text-align:center; padding:2em; color: black; font-color: black; background-color:white; border:2px solid red; position:absolute; left: 50%; top:50%; width:300px; height:300px; margin-left:-150px; margin-top:-90px\">" +
//                            "<span style=\"font-size:1.4em; font-weight:bold;\">SESSION AUTO LOGOFF ALERT!</span><br/><br/>" +
//                            "You will be automatically logged off.<br/><br/>" +
//                            "<span style=\"font-size:1.4em; font-weight:bold;\" id=\"countDown\"></span><br/><br/>" +
//                            "Click anywhere on the page to continue working." +
//                            "</div>")
//                            .appendTo($("body"));

//                        countdown = popup.find("#countDown");
//                    }

//                    popup.show();
//                    updateCountDown();
//                };

//                var resetTimeout = function () {
//                    // Reset timeout by "pinging" server.
//                    if (!pinging) {
//                        pinging = true;
//                        var pingTime = (new Date()).getTime();
//                        $.ajax({
//                            type: "GET",
//                            dataType: "json",
//                            url: pingUrl,
//                            success: function (serverTimeOutInSeconds) {
//                                // Stop countdown.
//                                window.clearTimeout(timer);
//                                if (popup) {
//                                    popup.hide();
//                                }

//                                // Subract time it took to do the ping from
//                                // the returned timeout and a little bit of
//                                // cushion so that client will be logged out
//                                // just before timeout has expired.
//                                // timeoutStartTime = (new Date()).getTime();
//                                //timeout = serverTimeOutInSeconds - (timeoutStartTime - pingTime) - cushion;
//                                timeout = serverTimeOutInSeconds;
//                                timeoutStartTime = serverTimeOutInSeconds;
//                                // Start warning timer.
//                                if (timeout <= 0)
//                                    window.location = redirectUrl;
//                                else if (timeoutStartTime > 0 && timeoutStartTime <= warningDuration) {
//                                    timer = window.setTimeout(showWarning, 1);
//                                }
//                                else {
//                                    timer = window.setTimeout(showWarning, (serverTimeOutInSeconds - warningDuration) * 1000);
//                                    timeoutStartTime = warningDuration;
//                                }


//                                pinging = false;
//                            },
//                            error: function (textStatus, errorThrown) {
//                                callbackfn("Error getting the data")
//                            }
//                        })
//                        //    .success(function (result) {


//                        //});
//                    }
//                };

//                // If user interacts with browser, reset timeout.
//                //$(document).on("mousedown mouseup keydown keyup", "", resetTimeout);
//                $(document).on("mousedown mouseup keydown keyup", "", resetTimeout);
//                $(window).resize(resetTimeout);

//                // Start fresh by reseting timeout.
//                resetTimeout();
//            },
//        };
//    }

//})(jQuery);

//diffIn(day,month,year,hour,minute,second)
//dateformat(DD-MM-YYYY h:m:s)

function dateDiffrenece(startDate, endDate, dateFormat, resultIn) {
    //startDate = "1/1/2023 12:10:10";
    //endDate = "1/1/2023 15:12:12";
    //dateFormat = "MM/DD/YYYY h:m:s";
    // diffIn = "hour";
    let lStartDate = moment(startDate, dateFormat);
    let lEndDate = moment(endDate, dateFormat);
    var result = lEndDate.diff(lStartDate, resultIn);
    //alert(result)
    return result;
}
var myspinner1 = '<div class="spinner-border" style="width: 8rem; height: 8rem; font-size: 40px; color: #0c2626"></div>';
var myspinner = '<div class="spinner-border spinner-border-lg text-primary" role="status"><span class="visually-hidden"> Loading...</span></div>';
function BlockUI() {
    
    $.blockUI({ message: myspinner });
}
function UnblockUI() {
    $.unblockUI();
}

function ActiveCurrentPageLink() {
    let path = window.location.pathname;
    let str = path;
    path = str.endsWith('/') ? str.slice(0, -1) : str;

    //path = "Shift"
    //$('ul.menu-inner').find('a[href="' + path + '"]').parent("li").addClass("active");
    //$('ul.menu-inner').find('a[href="' + path + '"]').parent("li").addClass("open");
    //$('ul.menu-inner').find('a[href="' + path + '"]').parent("li").parent("li").addClass("active");
    //$('ul.menu-inner').find('a[href="' + path + '"]').parent("li").parent("li").addClass("open");
    $('ul.menu-inner').find('a[href$="' + path + '"]').parents('li:eq(0)').addClass("active");
    $('ul.menu-inner').find('a[href$="' + path + '"]').parents('li:eq(0)').addClass("open");
    $('ul.menu-inner').find('a[href$="' + path + '"]').parents('li:eq(1)').addClass("active");
    $('ul.menu-inner').find('a[href$="' + path + '"]').parents('li:eq(1)').addClass("open");
}

function $successalert(title, message) {
    Swal.fire({
        title: title,
        text: message,
        icon: 'success',
        customClass: {
            confirmButton: 'btn btn-success'
        },
        buttonsStyling: false
    });
}
function $erroralert(title, message) {
    Swal.fire({
        title: title,
        text: message,
        icon: 'error',
        customClass: {
            confirmButton: 'btn btn-primary'
        },
        buttonsStyling: false
    });
}
function $htmlalert(alertType, title, html) {
    Swal.fire({
        title: "<strong>" + title + "</strong>",
        icon: alertType,
        html: html,
        showCloseButton: !0,
        showCancelButton: 0,
        focusConfirm: !1,
        confirmButtonText: 'OK',
        confirmButtonAriaLabel: "",
        //cancelButtonText: '',
        //cancelButtonAriaLabel: "",
        //customClass: {
        //    confirmButton: "btn btn-primary me-3",
        //    cancelButton: "btn btn-label-secondary"
        //},
        //buttonsStyling: !1
    })
}
function $warningalert(title, message) {
    Swal.fire({
        title: title,
        text: message,
        icon: "warning",
        customClass: {
            confirmButton: "btn btn-warning"
        },
        buttonsStyling: !1
    })
}

function validateImageFileTypeByFile(file) {
    let res = true;
    //const file = input.files[0];
    const validImageTypes = ["image/gif", "image/jpeg", "image/png"];
    if (file && !validImageTypes.includes(file.type)) {
        res = false;
        //alert("Please select a valid image file (JPEG, PNG, GIF).");
        //input.value = ""; // Clear the input
    }
    return res;
}

function validateImageFileTypeByInput(input) {
    const file = input.files[0];
    const validImageTypes = ["image/gif", "image/jpeg", "image/png"];
    if (file && !validImageTypes.includes(file.type)) {
        $erroralert("Invalid Image", "Please select a valid image file (JPEG, PNG, GIF).");
        input.value = ""; // Clear the input
    }
}

function notificationClicked(Id, redirectLink) {
    //$("#Header_NotificationId").val(Id);
    //window.location.href = redirectLink + "?encMessageId=" + Id;
    window.location.href = redirectLink + "?encMessageId=" + Id;
}

function fetchNotifications() {
    //var inputDTO = {
    //    "EmployeeAnnouncementFileUploadsId": EmployeeAnnouncementFileUploadsId
    //};
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Notification/GetNotificationsPartialView",
        //data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_PartialView_Notification').html(data);
            UnblockUI();

            let notificationCount = $("#NotificationCountInPartial").val();
            if (isNaN(notificationCount)) {
                $("#NotificationCount").hide();
            }
            else {
                if (parseInt(notificationCount) > 0) {
                    $("#NotificationCount").show();
                    $("#NotificationCount").text(notificationCount);
                }
                else {
                    $("#NotificationCount").hide();
                }
            }



        },
        error: function (result) {
            alert(result.responseText);
            UnblockUI();
        }
    });
}
function clearNotification(encMessageId) {
    var inputDTO = {
        "encMessageId": encMessageId
    };
    $.ajax({
        type: "POST",
        url: "/Notification/ClearNotification",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {          
            fetchNotifications();
        },
        error: function (error) {

        }
    });

}