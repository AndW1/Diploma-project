
$(document).ready(function () {

    $('.dropdown-trigger').dropdown();

    $(document).on('click', '#open_modal', function () {
        $('#error').css('display', 'none');

        var $list = $('.main input');

        var count = 0;

        for (var i = 0; i < $list.length; i++) {
            var checkbox = $list[i];
            if ($(checkbox).prop("checked") == true) {
                count++;
            }
        }

        if (count == 0) {
            var instance = M.Modal.getInstance($('.modal'));
            instance.close();
            $('#error').css('display', 'block');
            $('#error').text('Не выбран ни один получатель');
            return;
        }

    });//modal window

    $(document).on('click', '#checkbox', function () {
        $('#error').css('display', 'none');

    });

          

    $(document).on('click', '#select_all', function () {
        $('#error').css('display', 'none');


        var $list = $('.main input');

        for (var i = 0; i < $list.length; i++) {
            var checkbox = $list[i];
            $(checkbox).prop("checked", true);
        }

    });//select all users

    $(document).on('click', '#deselect_all', function () {
        $('#error').css('display', 'none');

        var $list = $('.main input');

        for (var i = 0; i < $list.length; i++) {
            var checkbox = $list[i];
            $(checkbox).prop("checked", false);
        }

    });//deselect all users

    $(document).on('click', '#send_email', function () {

        var instance = M.Modal.getInstance($('.modal'));
        instance.close();

        var message = $('#text_email').val();

        if (message == "") {
          
            $('#error').css('display', 'block');
            $('#error').text('Пустые письма не будут отправлены');
            return;
        }

      
        $('#error').css('display', 'none');

        var textEmail = $('#text_email').val();
        $('#text_email').val("");

        var $list = $('.main #checkbox');

        for (var i = 0; i < $list.length; i++) {

            var checkbox = $list[i];
            if ($(checkbox).prop("checked") == true) {

                var name = $(checkbox).parent().parent().parent().parent().parent().find('.header');
                var toastHTML = '<span>' + name.text() + '</span><button class="btn-flat toast-action">Отправлено</button>';  
                M.toast({ html: toastHTML});
             
                var url = "/Admin/SendEmailAsync";
                var id = $(checkbox).parent().find('#info').val();

                $.ajax({

                    url: url,
                    method: 'POST',
                    dataType: 'json',
                    data: {
                        "textEmail": textEmail,
                        "id": id,
                    },

                    success: function () {
                       
                    },

                });
            }
        }   

        for (var i = 0; i < $list.length; i++) {
            var checkbox = $list[i];
            $(checkbox).prop("checked", false);
        }

    });//send email click

});




    function functionChange() {
        $('#error').css('display', 'none');

        var num = $('#num').val();

        var url = "/Admin/FilterSearchAsync";

        $.ajax({

        url: url,
            method: 'POST',
            dataType: 'json',
            data: {
        "number": num,
            },

            success: function (obj) {
            
                if (obj == false) {
        window.location.href = '/Account/Signin';
    }

                var array = jQuery.parseJSON(obj);

                var container = $('#subresult');
                container.remove();


                var count_row = Math.ceil(array.length / 2);

                var html = "<div id='subresult'>";

                for (var i = 0; i < count_row; i++) {
            html += " <div class='row'>";

        for (var j = i * 2; j < i * 2 + 2; j++) {

                        if (j < array.length) {

                            var converted_date = convertDate(array[j].DateRegister);


                            html += "<div class='col s12 m6'>"
                            html += "<h4 class='header' style='color: darkblue'>" + array[j].Name + "   " + array[j].LastName + "</h4>";
                            html += "<div class='card horizontal'>";

                            html += "<div class='card-image' id='image_size'>";
                            if (array[j].Image != null && array[j].Image != "") {

                        html += "<img src='../images/" + array[j].Image + "'/>";
                    }
                            else {

                        html += "<img src='../images/icon/nophoto.png' />"
                    }
                    html += "</div>";


                            html += "<div class='card-stacked'>";//begin stacked
                            html += "<div class='card-content'>";//begin content

                            html += "<ul class='collapsible'>";

                            html += "<li>";
                            html += "<div class='collapsible-header'><i class='material-icons'>date_range</i>Дата регистрации</div>";
                            html += "<div class='collapsible-body'><span>" + converted_date+"</span></div >";
                            html += "</li>";


                            html += "<li>";
                            html += "<div class='collapsible-header'><i class='material-icons'>storage</i>Количество задач</div>";
                            html += "<div class='collapsible-body'><span>" + array[j].CountTask+"</span></div>";
                            html += "</li>";

                            html += "</ul>";
                            html += "</div>";//end content
                            html += "<div class='card-action'>";
                            html += "<label>";
                            html += "<input type='checkbox' id='checkbox' />";
                            html += "<span><i class='material-icons'>contact_mail</i></span>";
                            html += "<input type='hidden' value='" + array[j].Id +"' id='info' />";
                            html += "</label>";
                            html += "</div>";
                            html += "</div>";//end stacked


                            html += "</div>";
                            html +="</div>";
                        }
                    }

                    html += " </div>";
                }
                html += " </div>";


$("#result").append(html);
$('.collapsible').collapsible();
            },


error: function () {

}

        });
     
    }// selector onchanged evant -> send Ajax filter

function convertDate(date) {

    var date = new Date(Date.parse(date));

    var day = date.getUTCDate();
    var month = (date.getMonth() + 1);
    var year = date.getFullYear();

    if (day < 10) {
        day = "0" + date.getUTCDate();

    }
    if (month < 10) {
        month = "0" + (date.getMonth() + 1);
    }

    return day + "." + month + "." + date.getFullYear();
}// convert c# DateTime to JS datetime







