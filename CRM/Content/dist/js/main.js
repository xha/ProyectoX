
(function(){
	'use strict';
	document.addEventListener('DOMContentLoaded', function(){
	  
		var btn;
		$(document).ready(function(){
			$('#alerta').fadeIn();     
  			setTimeout(function() {
      		 $("#alerta").fadeOut();           
 			 },2000);
		});
		$(document).ready(function(){
			$('.eliminar').click(function(){
			   $(this).parent().parent().remove(); 
			   
			   getTotal();

			});
		});

		$(document).ready(function(){
			$('#cant-prod').keyup(function(){
			  var Subtotal=$(this).val()*$(this).closest("tr").find("td:eq(1)").html();
			  $(this).closest("tr").find("td:eq(3)").html(Subtotal.toFixed(2));
			 
			  getTotal();
			  

			});
		});
		function getTotal(){
			
				var total=0;
				$("tbody tr").each(function(){
					 total=total+Number($(this).find("td:eq(3)").html());
	
	
				});
		$("#total").html(total.toFixed(2)+"Bs");
			}
		
		$(document).ready(function getTotal(){
		{
			var total=0;
			$("tbody tr").each(function(){
				 total=total+Number($(this).find("td:eq(3)").html());


			});
			$("#total").html(total.toFixed(2)+"Bs");
		};
		});

		$(document).ready(function(){
			$('.to_register').click(function(){
				console.log("hola");
			   $('div .contenedor-login').addClass('hidden');
			   $('div .contenedor-registro').removeClass('hidden');
			});
		});
		$(document).ready(function(){
			$('.to_login').click(function(){
			   $('div .contenedor-registro').addClass('hidden');
			   $('div .contenedor-login').removeClass('hidden');
			});
		});


		$(document).ready(function(){
			
		
			$('#passwordsignup_confirm').blur(function(){

			
			 
			   var clave1 = document.getElementById("passwordsignup");
			   var clave2 = document.getElementById("passwordsignup_confirm");
		
			   if (clave1.value === clave2.value) {					
				  alert("Las dos claves son iguales...\nRealizaríamos las acciones del caso positivo") 	}					
			   else 		{				
				  alert("Las dos claves son distintas...") 
				  clave2.value=""
				  $('#passwordsignup_confirm').html(clave2.value);
				}
				
			});
		});

		
			
		  
		 
	});
	
  })();