



var counterProductDet = document.getElementById("counter-product");
var counterProduct1 = document.getElementById("counter-product1");
var counterProduct2 = document.getElementById("counter-product2");


var countDet = 1;
var count1 = 1;
var count2 = 1;

function PluscounterDet() {

    countDet++;
    counterProductDet.innerHTML = countDet;
};
function minuscounterDet() {

    countDet = countDet - 1;
    if (countDet <= 1) {
        countDet = 1;
    }
    counterProductDet.innerHTML = countDet;
}


function Pluscounter1() {

    count1++;
    counterProduct1.innerHTML = count1;
};
function minuscounter1() {

    count1 = count1 - 1;
    if (count1 <= 1) {
        count1 = 1;
    }
    counterProduct1.innerHTML = count1;
}

function Pluscounter2() {

    count2++;
    counterProduct2.innerHTML = count2;
};
function minuscounter2() {

    count2 = count2 - 1;
    if (count2 <= 1) {
        count2 = 1;
    }
    counterProduct2.innerHTML = count2;
}