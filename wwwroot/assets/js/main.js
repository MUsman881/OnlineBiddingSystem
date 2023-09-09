(function ($) {
    "use strict";
    jQuery(window).on('load', function () {
        $(".preloader").delay(1600).fadeOut("slow");
    });
    $('select').niceSelect();
    setTimeout(myGreeting, 1800);

    function myGreeting() {
        var wow = new WOW({
            boxClass: 'wow',
            animateClass: 'animated',
            offset: 0,
            mobile: true,
            live: true,
            callback: function (box) { },
            scrollContainer: null,
            resetAnimation: true,
        });
        wow.init();
    }
    window.addEventListener('scroll', function () {
        const header = document.querySelector('header.style-1, header.style-2, header.style-3');
        header.classList.toggle("sticky", window.scrollY > 0);
    });
    $(window).on('scroll', function () {
        if ($(window).scrollTop() > 300) {
            $('.scroll-btn').addClass('show');
        } else {
            $('.scroll-btn').removeClass('show');
        }
    });
    $('.scroll-btn').on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: 0
        }, '300');
    });
    $('.mobile-menu-btn').on("click", function () {
        $('.main-menu').addClass('show-menu');
    });
    $('.menu-close-btn').on("click", function () {
        $('.main-menu').removeClass('show-menu');
    });
    $('.dropdown-icon').on('click', function () {
        $(this).toggleClass('active').next('ul').slideToggle();
        $(this).parent().siblings().children('ul').slideUp();
        $(this).parent().siblings().children('.active').removeClass('active');
    });
    var toggleIcon = document.querySelectorAll('.sidebar-menu-icon')
    var closeIcon = document.querySelectorAll('.cross-icon')
    var searchWrap = document.querySelectorAll('.menu-toggle-btn-full-shape')
    toggleIcon.forEach((element) => {
        element.addEventListener('click', () => {
            document.querySelectorAll('.menu-toggle-btn-full-shape').forEach((el) => {
                el.classList.add('show-sidebar')
            })
        })
    })
    closeIcon.forEach((element) => {
        element.addEventListener('click', () => {
            document.querySelectorAll('.menu-toggle-btn-full-shape').forEach((el) => {
                el.classList.remove('show-sidebar')
            })
        })
    })
    window.onclick = function (event) {
        searchWrap.forEach((el) => {
            if (event.target === el) {
                el.classList.remove('show-sidebar')
            }
        })
    }
    var heroSliderTwo = new Swiper('.banner1', {
        slidesPerView: 1,
        speed: 1500,
        loop: true,
        spaceBetween: 10,
        loop: true,
        centeredSlides: true,
        roundLengths: true,
        parallax: true,
        effect: 'fade',
        navigation: false,
        fadeEffect: {
            crossFade: true,
        },
        autoplay: {
            delay: 4000
        },
        pagination: {
            el: ".hero-one-pagination",
            clickable: true,
        }
    });
    var swiper = new Swiper(".category1-slider", {
        slidesPerView: 1,
        speed: 1000,
        spaceBetween: 30,
        loop: true,
        autoplay: true,
        roundLengths: true,
        navigation: {
            nextEl: '.category-prev1',
            prevEl: '.category-next1',
        },
        breakpoints: {
            280: {
                slidesPerView: 1
            },
            440: {
                slidesPerView: 2
            },
            576: {
                slidesPerView: 2
            },
            768: {
                slidesPerView: 3
            },
            992: {
                slidesPerView: 5
            },
            1200: {
                slidesPerView: 6
            },
            1400: {
                slidesPerView: 7
            },
        }
    });

    $('#slick1').slick({
        rows: 2,
        dots: true,
        arrows: false,
        infinite: true,
        speed: 300,
        slidesToShow: 6,
        slidesToScroll: 6,
        responsive: [{
            breakpoint: 1200,
            settings: {
                arrows: false,
                slidesToShow: 5
            }
        }, {
            breakpoint: 991,
            settings: {
                arrows: false,
                slidesToShow: 4
            }
        }, {
            breakpoint: 768,
            settings: {
                arrows: false,
                slidesToShow: 3
            }
        }, {
            breakpoint: 576,
            settings: {
                arrows: false,
                slidesToShow: 2
            }
        }, {
            breakpoint: 480,
            settings: {
                arrows: false,
                slidesToShow: 2
            }
        }, {
            breakpoint: 350,
            settings: {
                arrows: false,
                slidesToShow: 1
            }
        }]
    });

    function makeTimer() {
        var endTime = new Date("July 25, 2023 00:00:00");
        var endTime = (Date.parse(endTime)) / 1000;
        var now = new Date();
        var now = (Date.parse(now) / 1000);
        var timeLeft = endTime - now;
        var days = Math.floor(timeLeft / 86400);
        var hours = Math.floor((timeLeft - (days * 86400)) / 3600);
        var minutes = Math.floor((timeLeft - (days * 86400) - (hours * 3600)) / 60);
        var seconds = Math.floor((timeLeft - (days * 86400) - (hours * 3600) - (minutes * 60)));
        if (hours < "10") {
            hours = "0" + hours;
        }
        if (minutes < "10") {
            minutes = "0" + minutes;
        }
        if (seconds < "10") {
            seconds = "0" + seconds;
        }
        $("#timer #days").html(days);
        $("#timer #hours").html(hours);
        $("#timer #minutes").html(minutes);
        $("#timer #seconds").html(seconds);
    }
    setInterval(function () {
        makeTimer();
    }, 1000);
    
    const togglePassword = document.querySelector('togglePassword');
    const password = document.querySelector('#Password');
    if (togglePassword != null) {
        togglePassword.addEventListener('click', function (e) {
            const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
            password.setAttribute('type', type);
            this.classList.toggle('bi-eye');
        });
    }
    const togglePassword2 = document.getElementById('togglePassword2');
    const password2 = document.querySelector('#ConfirmPassword');
    if (togglePassword2) {
        togglePassword2.addEventListener('click', function (e) {
            const type = password2.getAttribute('type') === 'password' ? 'text' : 'password';
            password2.setAttribute('type', type);
            this.classList.toggle('bi-eye');
        });
    }
    $(".counter-item").each(function () {
        $(this).isInViewport(function (status) {
            if (status === "entered") {
                for (var i = 0; i < document.querySelectorAll(".odometer").length; i++) {
                    var el = document.querySelectorAll('.odometer')[i];
                    el.innerHTML = el.getAttribute("data-odometer-final");
                }
            }
        });
    });
    $(".counter-single").each(function () {
        $(this).isInViewport(function (status) {
            if (status === "entered") {
                for (var i = 0; i < document.querySelectorAll(".odometer").length; i++) {
                    var el = document.querySelectorAll('.odometer')[i];
                    el.innerHTML = el.getAttribute("data-odometer-final");
                }
            }
        });
    });

}(jQuery));