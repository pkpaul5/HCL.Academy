	
	$(document).ready(function(){
		$('.btn-login').click(function(){
			$('body').addClass('fade-in');
		});

	    /*--------- Tab ---------*/
		$(".nav-justified a").click(function (e) {
		    e.preventDefault();
		    $(this).tab('show');
		});
	    /*--------- Tab ---------*/

	    $("#btnSearch").click(function () {       

	        if ($.trim($("#keyword").val()).length == 0) {
	            alert("Please enter a keyword");

	            $("#keyword").focus();
	            return false;
	        }
	    });	 
	        
		 
	});

	//$(document).ready(function () {
	//    $('#btnSearch').click(function (e) {
	//        if ($('#keyword').val()) {
	//            //alert('hi');
	//        }
	//        else {
	//            e.preventDefault();
	//        }
	//    });
	//});

	

	