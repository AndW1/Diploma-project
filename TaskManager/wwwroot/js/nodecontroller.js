

$(document).ready(function () {


    $(document).on('click', '#createnode', function () {

        var node = document.getElementById('create_node'); // находим наше "окно"
        node.style.display = 'block';

        var er = document.getElementById('error');
        er.textContent = "";

    });//button add node-> open create node window


    $(document).on('click', '#cancel', function () {

        var node = document.getElementById('create_node'); // находим наше "окно"

        node.style.display = 'none';

        $('#content').val('');
    });//close create window



    $(document).on('click', '#savenode', function () {

        var node = document.getElementById('create_node'); // находим наше "окно"
        node.style.display = 'none';

        var idOwner = $('#id').val();
        var content = $('#content').val();

        if (content == "" || content == null) {
            var er = document.getElementById('error');
            er.textContent = "EMPTY CONTENT FIELD!";
            return;
        }

        var url = "/NodeStages/CreateNodAsync";
        $.ajax({
            url: url,
            method: 'POST',
            dataType: 'json',
            data: {
                "idOwner": idOwner,
                "content": content
            },
            success: function (node) {
                if (node == false) {
                    window.location.href = '/Account/Signin';
                }


                var node_items = $("div#nodeitem");

                if (node_items.length < 10) {

                    var obj = jQuery.parseJSON(node);

                   
                    

                    var html = " <div class='collection-item' id='nodeitem'>";
                    html += "<span class='badge' onclick='removeNode(" + obj.Id + ")'><a href='#!/><i class='material-icons'>&#10006;</i></a></span>";
              
                    html += "<label>";
                    html += "<input type ='checkbox' id='check'" + "value='" + obj.Id + "'" + " > <span></span>" + "</label>";
                    html += "<span id='contentnode'>" + obj.ContentNode + "</span>";

                    html += "</label";
                    html += "</div>";

                    $(".result").append(html);
          
                }
                redrawPaginationPlus();
                $('#content').val('');
            },
        });
    });//save new node


    $(document).on('click', '.close', function () {

        var button = $(this);

        var idNode = $(this).val();
        var url = "/NodeStages/DeleteNodeAsync";


        $.ajax({

            url: url,
            method: 'POST',
            dataType: 'json',
            data: {
                "idNode": idNode
            },

            success: function (e) {

                if (e == false) {
                    window.location.href = '/Account/Signin';
                }

                button.parent().remove();
                redrawPaginationMinus();

            }
        });
    });//remove selected node


    $(document).on('click', '#check', function () {

        if ($(this).prop('checked')) {

            var idNode = $(this).val();

            var checkbox = $(this);

            var url = "/NodeStages/SetLineAsync";

            $.ajax({

                url: url,
                method: 'POST',
                dataType: 'json',
                data: {
                    "idNode": idNode
                },

                success: function (e) {
                    if (e == false) {
                        window.location.href = '/Account/Signin';
                    }
                    checkbox.parent().parent().find('#contentnode').css("text-decoration", "line-through");
                }
            });
        }
        else {

            var idNode = $(this).val();

            var checkbox = $(this);

            var url = "/NodeStages/RemoveLineAsync";

            $.ajax({

                url: url,
                method: 'POST',
                dataType: 'json',
                data: {
                    "idNode": idNode
                },

                success: function (e) {
                    if (e == false) {
                        window.location.href = '/Account/Signin';
                    }

                    checkbox.parent().parent().find('#contentnode').css("text-decoration", "none");
                }
            });
        }

    });//set line for checked node


    $(document).on('click', '#pagenum', function () {

        var elem = $(this).parent().find('.active');
        elem.removeClass('active');
        $(this).addClass('active');
        pagination($(this).val());


        if ($(this).val() == 1) {
            var left = $(this).parent().find('#left');
            left.addClass('disabled');

            var right = $(this).parent().find('#right');
            if (right.hasClass('disabled')) {
                right.removeClass('disabled');
               // right.addClass('waves-effect');
            }

        }

        var count = $(this).parent().children('#pagenum').length;

        if ($(this).val() == count) {

            var right = $(this).parent().find('#right');
            right.addClass('disabled');

            var left = $(this).parent().find('#left');

            if (left.hasClass('disabled')) {
                left.removeClass('disabled');
            }

        }

        if ($(this).val() > 1 && $(this).val() < count) {

            var left = $(this).parent().find('#left');

            if (left.hasClass('disabled')) {
                left.removeClass('disabled');
            }

            var right = $(this).parent().find('#right'); {
                if (right.hasClass('disabled')) {
                    right.removeClass('disabled');
                }

            }
        }

    });// pagination buttons class => active ore disabled


    $(document).on('click', '#left', function () {

        if (!$(this).hasClass('disabled')) {

            var button = $(this).parent().find('.active').prev();

            if (button.val() >= 1) {
                var next = $(this).parent().find('.active');
                next.removeClass('active');
                button.addClass('active');

                var right = $(this).parent().find('#right');
                if (right.hasClass('disabled')) {

                    right.removeClass('disabled');

                }
            }

            if (button.val() == 1) {
                $(this).addClass('disabled');

            }

            pagination(button.val());

        }
    });//left button click pagination


    $(document).on('click', '#right', function () {

        if (!$(this).hasClass('disabled')) {

            var button = $(this).parent().find('.active').next();

            var count = $(this).parent().children('#pagenum').length;

            if (button.val() <= count) {

                var prev = $(this).parent().find('.active');
                prev.removeClass('active');

                button.addClass('active');

                var left = $(this).parent().find('#left');

                if (left.hasClass('disabled')) {

                    left.removeClass('disabled');
                }
            }

            if (button.val() == count) {
                var right = $(this).parent().find('#right');
                right.addClass('disabled');
            }

            pagination(button.val());

        }

    });//right button click pagination
});


function pagination(number) {

    if (number == null) {
        return false;
    }

   
    var id = $('#id').val();

    var url = "/NodeStages/PaginationJsonAsync";

    $.ajax({
        url: url,
        method: 'POST',
        dataType: 'json',
        data: {
            "pagenumber": number,
            "id": id,
        },
        success: function (array) {

            if (array == false) {
                window.location.href = '/Account/Signin';
            }

            var node_items = $("div#nodeitem");

            $(node_items).remove();

            var obj = jQuery.parseJSON(array);
            var len = obj.length;

            for (var i = 0; i < obj.length; i++) {


                var html = "<div class='collection-item' id='nodeitem'>";
            
                html += "<span class='badge' onclick='removeNode(" + obj[i].Id +")'><a href='#!/><i class='material-icons'>&#10006;</i></a></span>";

        
                html += "<label>";
                var f = obj[i].NodeCreated;

                if (f == true) {
                    html += "<input type ='checkbox' id='check' checked='checked' " + "value='" + obj[i].Id + "'" + " > <span></span>" + "</label>";
                }
                else {
                    html += "<input type ='checkbox' id='check'" + "value='" + obj[i].Id + "'" + " > <span></span>" + "</label>";
                }

                if (f == true) {

                    html += "<span id='contentnode' style='text-decoration:line-through'>" + obj[i].ContentNode + "</span>";
                }
                else {
                    html += "<span id='contentnode'>" + obj[i].ContentNode + "</span>";
                }
             
                html += "</label";
                html += "</div>";

                $(".result").append(html);

            }

        },
        error: function () {
            //alert('error');
        }
    });
}//Ajax request for page number
 
function redrawPaginationPlus() {

    var id = $('#id').val();

    var url = "/NodeStages/GetTotalPageAsync";

    $.ajax({
        type: "GET",
        url: url,
        dataType: 'json',
        data: {

            "idOwner": id
        },
        success: function (totalPages) {

            if (totalPages == false) {
                window.location.href = '/Account/Signin';
            }

            var $list = $('.pagination li');

            if (totalPages > ($list.length - 2)) {

                var ul = $('.pagination');
                ul.remove();

                var num = $list.length - 2;
            
                drawPaginationPlus(totalPages);
            }
        }
    });
}


function drawPaginationPlus(num) {

    if (num > 1) {

        var html = "<ul class='pagination'><li' id='left'><a href='#!'><i class='material-icons'>chevron_left</i></a></li>";
        html += "<li value ='1' id ='pagenum'><a href='#!'>1</a></li >";

        for (var i = 1; i < num; i++) {
            var index = i + 1;
            if (index != num) {
                html += "<li value='" + index + "' id='pagenum'><a href='#!'>" + index + "</a></li>";
            }
            else {
                html += "<li class='active' value='" + index + "' id='pagenum'><a href='#!'>" + index + "</a></li>";
            }

        }
        html += "<li class='disabled' id='right'><a href='#!'><i class='material-icons'>chevron_right</i></a></li ></ul>";

        $(".ul").append(html);

        pagination(num);
    }
}


function redrawPaginationMinus() {

    var pagenumber = $('ul .active');
   
    var $list = $('.pagination li');

    if ($list.length == 3) {

        var node_items = $("div#nodeitem");

        if (node_items.length > 0 && node_items.length <= 10) {
     
            pagination(pagenumber.val());
            
            drawPaginationMinus(pagenumber.val());
        }
        else if (node_items.length == 0) {
          
            pagination($list.length - 2);
            drawPaginationMinus($list.length - 2);
        }

    }
    if ($list.length > 3) {
       var pagenumber = $('ul .active');
       
        var c = $("div#nodeitem");
      
        if (c.length > 0 && c.length <= 10) {

            pagination(pagenumber.val());
        
            drawPaginationMinus(pagenumber.val());
        }
        else if (c.length == 0) {
            pagination(pagenumber - 1);
            drawPaginationMinus(pagenumber - 1);
        }
    }
    if ($list.length == 0) {

        var node_items = $("div#nodeitem");
        if (node_items.length > 0 && node_items.length <= 10) {
            pagination(1);        
        }
        else {
            return;
        }
    }
}


function drawPaginationMinus(num) {

    var id = $('#id').val();

    var url = "/NodeStages/GetTotalPageAsync";

    $.ajax({
        type: "GET",
        url: url,
        dataType: 'json',
        data: {

            "idOwner": id
        },
        success: function (totalPages) {

            if (totalPages == false) {
                window.location.href = '/Account/Signin';
            }

            if (totalPages > 1) {

                var pagenumber = $('ul .active');

                var $list = $('.pagination li');

                var ul = $('.pagination');
                ul.remove();


                var html = "<ul class='pagination'>";

                var n;

                if (pagenumber.val() < totalPages) {
                    n = pagenumber.val();
                }
                if (pagenumber.val() == totalPages) {
                    n = pagenumber.val();
                }
                if (pagenumber.val() > totalPages) {
                    n = pagenumber.val() - 1;
                }

                if (n == 1) {
                    html += "<li class='disabled' id='left'><a href='#!'><i class='material-icons'>chevron_left</i></a></li>";
                }
                else {
                    html += "<li id='left'><a href='#!'><i class='material-icons'>chevron_left</i></a></li>";
                }

                for (var i = 0; i < totalPages; i++) {
                    var index = i + 1;

                    if (index != n) {
                        html += "<li value='" + index + "' id='pagenum'><a href='#!'>" + index + "</a></li>";
                    }
                    else {
                        html += "<li class='active' value='" + index + "' id='pagenum'><a href='#!'>" + index + "</a></li>";
                    }

                }
                if (n == totalPages) {
                    html += "<li class='disabled' id='right'><a href='#!'><i class='material-icons'>chevron_right</i></a ></li ></ul>";
                }
                else {
                    html += "<li id='right'><a href='#!'> <i class='material-icons'>chevron_right</i></a ></li ></ul>";
                }

                $(".ul").append(html);

                pagination(n);

            }
            else {
                var ul = $('.pagination');
                ul.remove();
                pagination(totalPages);
            }
        }

    });
}



function removeNode(idNode) {
   
    var url = "/NodeStages/DeleteNodeAsync";

    $.ajax({

        url: url,
        method: 'POST',
        dataType: 'json',
        data: {
            "idNode": idNode
        },

        success: function (e) {

            if (e == false) {
                window.location.href = '/Account/Signin';
            }

            redrawPaginationMinus();
        }
    });
}//Ajax remove node

