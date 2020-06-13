        $(function () {



            draggableInit();
            
            
        });

        function getTotal() {
            var decimal_places = 2;
            var decimal_factor = decimal_places === 0 ? 1 : Math.pow(10, decimal_places);
            var totalDolares = 0;
            var totalSoles = 0;
            $(".panel-body").each(function () {

                $(this).find(".oportunidad").each(function () {
                    if ($(this).find('#simboloMon').html() === '$.') {
                        totalDolares = totalDolares + parseFloat($(this).find("#ingreso").html());
                    } else {
                        totalSoles = totalSoles + parseFloat($(this).find("#ingreso").html());
                    }
                });
                $(this).parent().find(".MontoDolar span").animateNumber({
                    number: totalDolares.toFixed(2) * decimal_factor,

                    numberStep: function (now, tween) {
                        var floored_number = Math.floor(now) / decimal_factor,
                            target = $(tween.elem);

                        if (decimal_places > 0) {
                            // coloca los decimales asi sean 0
                            floored_number = floored_number.toFixed(decimal_places);

                            // cambia '.' por la  ','

                            //floored_number = floored_number.toString().replace(',', '.');
                        }

                        target.text(floored_number);
                    }

                }, 1200);
                totalDolares = 0;

                $(this).parent().find(".MontoSoles span").animateNumber({
                    number: totalSoles.toFixed(2) * decimal_factor,

                    numberStep: function (now, tween) {
                        var floored_number = Math.floor(now) / decimal_factor,
                            target = $(tween.elem);

                        if (decimal_places > 0) {
                            // coloca los decimales asi sean 0
                            floored_number = floored_number.toFixed(decimal_places);

                            // cambia '.' por la  ','
                            //floored_number = floored_number.toString().replace('.', ',');
                        }

                        target.text(floored_number);
                    }

                }, 1200);
                totalSoles = 0;
            });

        }



        function draggableInit() {
            var panelInicio;
            var sourceId;
            var idOportunidad;
            $('[draggable=true]').bind('dragstart', function (event) {

                console.log($(this).children());
                panelInicio = $(this).parent();
                sourceId = $(this).parent().attr('id');
                idOportunidad = $(this).attr('id');
                console.log('inicio: ' + sourceId);
                console.log('Oportunidad: ' + idOportunidad);
                event.originalEvent.dataTransfer.setData("text/plain", event.target.getAttribute('id'));
            });
            $('.panel-body').bind('dragover', function (event) {
                event.preventDefault();
            });
            // colocar en otra columna
            $('.panel-body').bind('drop', function (event) {
                console.log($(this).children())
                var panelFinal = $(this).children();
                var children = $(this).children();
                var targetId = children.attr('id');
                console.log('Objetivo: '+targetId);

                if (sourceId !== targetId) {
                    var data = { estadoInicial: sourceId, estadoFinal: targetId, idOportunidad: idOportunidad};
                    
                    $.post(urlcambiarEst, data).done(function (data) {
                        
                        // despues de enviar

                        var element = document.getElementById(elementId);
                        children.prepend(element);
                        getTotal() ;


                        toastr.success("Cambio de estado realizado con exito");
                       

                    }).fail(function () {
                        // despues de enviar
                        setTimeout(function () {
                            
                            getTotal();

                            toastr.error("Ha ocurrido un error.")
                        }, 1000);

                    })
                    var elementId = event.originalEvent.dataTransfer.getData("text/plain");
                    


                    

                }

                event.preventDefault();
            });
        }


        function CambiarPosicion() {
            var posicionInicio;
            var sourceIdEstado;
            
            $('[estadoDraggable=true]').bind('dragstart', function (event) {
                posicionInicio = $(this).parent().parent().attr('id');
                sourceIdEstado = $(this).attr('id');
                
                console.log('posicion inicio : ' + posicionInicio);
                console.log('Estado de Oportunidad: ' + sourceIdEstado);
                event.originalEvent.dataTransfer.setData("text/plain", event.target.getAttribute('id'));
            });
            // al pasar en elemento por encima de la columna
            $('.columna').bind('dragover', function (event) {
                event.preventDefault();
            });
            // colocar en otra columna
            $('.columna').bind('drop', function (event) {
                var posicionFinal = $(this).attr('id');
                var children = $(this).children().children();
                var targetIdEstado = children.attr('id');
                console.log('posicion final : ' + posicionFinal);
                console.log('Objetivo: ' + targetIdEstado);

                if (sourceIdEstado !== targetIdEstado) {
                    var data = { posicionInicial: posicionInicio, posicionFinal: posicionFinal, idEstadoInicial: targetIdEstado, idEstadoFinal: sourceIdEstado };

                    $.post(urlcambiarPosicion, data).done(function (data) {

                        // despues de enviar

                            var element = document.getElementById(elementId);
                            children.prepend(element);
                            getTotal();

                            toastr.success("Cambio de estado realizado con exito");
 

                    }).fail(function () {
 

                            getTotal();


                            toastr.error("Ha ocurrido un error.")


                    })
                    var elementId = event.originalEvent.dataTransfer.getData("text/plain");





                }

                event.preventDefault();
            });
        }