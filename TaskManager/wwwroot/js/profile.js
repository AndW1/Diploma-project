


$(document).ready(function () {

    var f = true;

    $(document).on('click', '#attach_file', function () {

        if (f == true) {
            var node = document.getElementById('inputfile'); // находим наше "окно"
            node.style.display = 'block';
            f = false;
        }
        else {
            var node = document.getElementById('inputfile'); // находим наше "окно"
            node.style.display = 'none';
            f = true;
        }
      
    });

});