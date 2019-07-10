function showModalWin() {

    var darkLayer = document.createElement('div'); // слой затемнения
    darkLayer.id = 'shadow'; // id чтобы подхватить стиль
    document.body.appendChild(darkLayer); // включаем затемнение

    var modalWin = document.getElementById('popupWin'); // находим наше "окно"
    modalWin.style.display = 'block'; // "включаем" его

   

    darkLayer.onclick = function () {  // при клике на слой затемнения все исчезнет
        darkLayer.parentNode.removeChild(darkLayer); // удаляем затемнение
        modalWin.style.display = 'none'; // делаем окно невидимым

       
        return false;
    };
}

function showEditWin() {

    var darkLayer = document.createElement('div'); // слой затемнения
    darkLayer.id = 'shadow'; // id чтобы подхватить стиль
    document.body.appendChild(darkLayer); // включаем затемнение

    var winEdit = document.getElementById('winEdit'); // находим наше "окно"
    winEdit.style.display = 'block'; // "включаем" его

    darkLayer.onclick = function () {  // при клике на слой затемнения все исчезнет
        darkLayer.parentNode.removeChild(darkLayer); // удаляем затемнение
        winEdit.style.display = 'none'; // делаем окно невидимым
        return false;
    };
}



$(document).ready(function () {

    $(document).on('click', '#delete_task', function () {

        showModalWin();

    });

    $(document).on('click', '#edit_task', function () {

        var er = document.getElementById('err');
        er.textContent = "";

        showEditWin();

    });


});


function functionDate() {

    var er = document.getElementById('err');

    if ($('#dc').val() > $('#df').val()) {

        er.textContent = "Дата окончния меньше текущей";
    }
    else {
        er.textContent = "";
    }
}
