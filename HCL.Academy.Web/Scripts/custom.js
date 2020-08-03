/*-------------------------------------------------------------------------
 * RENDIFY - Custom jQuery Scripts
 * ------------------------------------------------------------------------

	1.	Plugins Init
	2.	Site Specific Functions
	3.	Shortcodes
	4.      Other Need Scripts (Plugins config, themes and etc)
	
-------------------------------------------------------------------------*/
"use strict";

jQuery(document).ready(function($){
	
    
/*------------------------------------------------------------------------*/
/*	1.	Plugins Init
/*------------------------------------------------------------------------*/


	/************** Single Page Nav Plugin *********************/
	$('.menu').singlePageNav(
		{filter: ':not(.external)'}
	);
      

	/************** FlexSlider Plugin *********************/
	$('.flexslider').flexslider({
		animation : 'fade',
		controlNav : false,
		nextText : '',
		prevText : '',
	});

	$('.flex-caption').addClass('animated bounceInDown');

	$('.flex-direction-nav a').on('click', function() {
        $('.flex-caption').removeClass('animated bounceInDown');
        $('.flex-caption').fadeIn(0).addClass('animated bounceInDown');
    });


	/************** LightBox *********************/
	$(function(){
		$('[data-rel="lightbox"]').lightbox();
	});




/*------------------------------------------------------------------------*/
/*	2.	Site Specific Functions
/*------------------------------------------------------------------------*/


	/************** Go Top *********************/
	$('#go-top').click(function(event) {
        event.preventDefault();
        jQuery('html, body').animate({scrollTop: 0}, 800);
        return false;
    });



    /************** Responsive Navigation *********************/
	$('.toggle-menu').click(function(){
        $('.menu').stop(true,true).toggle();
        return false;
	});

    $(".responsive-menu .menu a").click(function(){
        $('.responsive-menu .menu').hide();
    });


    var str = window.location.href;
    var val = str.lastIndexOf('/');
    var myVal = getPosition(str, '/', 3)
    var value = window.location.href.substring(myVal + 1, val);
    //alert(myVal);
    //alert(value2);
    //alert(value);
    var word = 'Customer#'
    var re = new RegExp('[^<\\/](' + word + ')', 'g');
    $('.section-cotent,.site-footer').each(function () {
        $(this).html($(this).html().replace(re, value));
    });

    function getPosition(str, m, i) {
        return str.split(m, i).join(m).length;
    }

});

function GetOnBoardReport() {
 
    $("#divAcademyOnboardingReports").html(" <img src='/Images/loading_01.gif' class='loader-img loader-position' />");
    var project = $('#project').val();
    var role = $('#ddRoleReport').val();
    var geo = $('#ddGEOReport').val();
    $.ajax({
        type: "POST",
        url: "/Admin/UserOnBoardingReport",
        data: {
            project: project,
            roleId: role,
            geoId: geo
        },
        success: function (result) {
            $("#divAcademyOnboardingReports").html(result);
        }
    })
}

function addRequestVerificationToken(data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};