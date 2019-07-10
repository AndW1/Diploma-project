
    $(window).on('resize', function () {
            var win = $(this); //this => window
            if (win.width() > 320 &&  win.width() <= 420) {
        $('.brand-logo').css('font-size', "20px");

    }
            if (win.width() >420) {
        $('.brand-logo').css('font-size', "40px");

    }
            if (win.width() <= 320) {
        $('.brand-logo').css('font-size', "15px");
    }
        });

        $(document).ready(function () {

        $('.sidenav').sidenav();
    $('.collapsible').collapsible();
            $('.modal').modal();
            $(".dropdown-trigger").dropdown();
            $('.materialboxed').materialbox();
            $('select').formSelect();

            var w = $(window).width();
            if (w>320 && w <=420) {
        $('.brand-logo').css('font-size', "20px");

    }
            if (w > 420) {
        $('.brand-logo').css('font-size', "40px");
    }
            if (w <= 320) {
        $('.brand-logo').css('font-size', "15px");
    }

});

        $('.carousel.carousel-slider').carousel({fullWidth: true });


