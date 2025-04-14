/*!===================================================================================
Template Name: Contractor
Author: LepordTheme
Version: 1.0.0
Created: April 2022
===================================================================================*/

/*====================================
[ JS TABLE CONTENT ]
------------------------------------
1.0 PROGRESS ICON
2.0 STICKEY HEADER
3.0 MOBILE MENU COLLAPSE
4.0 CLIENT SLIDER
5.0 BRAND SLIDER
6.0 FORM VALIDATOR
-------------------------------------
[ END Js TABLE CONTENT ]
=====================================*/

(function($) {
    "use strict";

/* =============================================
1.0 PROGRESS ICON
============================================= */
var progressPath = document.querySelector('.progress-wrap path');
var pathLength = progressPath.getTotalLength();
progressPath.style.transition = progressPath.style.WebkitTransition = 'none';
progressPath.style.strokeDasharray = pathLength + ' ' + pathLength;
progressPath.style.strokeDashoffset = pathLength;
progressPath.getBoundingClientRect();
progressPath.style.transition = progressPath.style.WebkitTransition = 'stroke-dashoffset 10ms linear';
var updateProgress = function() {
    var scroll = $(window).scrollTop();
    var height = $(document).height() - $(window).height();
    var progress = pathLength - (scroll * pathLength / height);
    progressPath.style.strokeDashoffset = progress;
}
updateProgress();
$(window).scroll(updateProgress);
var offset = 150;
var duration = 550;
jQuery(window).on('scroll', function() {
    if (jQuery(this).scrollTop() > offset) {
        jQuery('.progress-wrap').addClass('active-progress');
    } else {
        jQuery('.progress-wrap').removeClass('active-progress');
    }
});
jQuery('.progress-wrap').on('click', function(event) {
    event.preventDefault();
    jQuery('html, body').animate({
        scrollTop: 0
    }, duration);
    return false;
})
/* =============================================
2.0 STICKEY HEADER 
============================================= */
$(window).scroll(function() {
    if($(this).scrollTop() > 80) {
        $('header').addClass("sticky");
    } else {
        $('header').removeClass("sticky");
    }
});
/* =============================================
3.0 MOBILE MENU COLLAPSE
============================================= */
$(".navbar-nav .nav-link").on('click', function(){
    $(".navbar-collapse").removeClass("show");
});
/* =============================================
4.0 CLIENT SLIDER
============================================= */
$('#client-slider').owlCarousel({
    loop: true,
    dots: true,
    nav: false,
    center: true,
    autoplay: true,
    animateIn:true,
    autoplayHoverPause: true,
    autoHeight: true,
    responsiveClass: true,
    items: 1,
});
/* =============================================
5.0 BRAND SLIDER
============================================= */
$('#brand-slider').owlCarousel({
    loop: true,
    dots: false,
    nav: false,
    autoplay: true,
    autoplayHoverPause: true,
    autoHeight: true,
    responsiveClass: true,
    responsive: {
        0: {
            items: 2
        },
        540: {
            items: 2,
        },
        600: {
            items: 3,
        },
        992: {
            items: 4,
        },
        1140: {
            items: 6,
        },
    }
});
/* =============================================
6.0 FORM VALIDATOR
============================================= */
 /* =============================================
         Form Validator
        ============================================= */
        // Example starter JavaScript for disabling form submissions if there are invalid fields
          (function () {
      'use strict';
      window.addEventListener('load', function () {
         var forms = document.getElementsByClassName('needs-validation');
         var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
               event.preventDefault();
               event.stopPropagation();
               if (form.checkValidity() !== false) {
                  var url = "php/contact.php";
                  $.ajax({
                     type: "POST",
                     url: url,
                     data: $(this).serialize(),
                     success: function (data) {
                        var messageAlert = 'alert-' + data.type;
                        var messageText = data.message;
                        var alertBox = '<div class="alert mb-40 ' + messageAlert + ' alert-dismissible fade show" role="alert">' + messageText + '<button type="button" class="close" data-bs-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></div>';
                        if (messageAlert && messageText) {
                           $('.needs-validation').find('.messages').hide().html(alertBox).fadeIn('slow');
                           $('.needs-validation')[0].reset();
                        }
                     }
                  });
               }
               form.classList.add('was-validated');
            }, false);
         });
      }, false);
   })();       
   })();
}(jQuery));




























