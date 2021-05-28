var i, m, currentYear, startYear, endYear, newOption, dropdownYear, dropdownMonth, currentMonth;
dropdownYear = document.getElementById("dropdownYear");
dropdownMonth = document.getElementById("dropdownMonth");
currentYear = (new Date()).getFullYear();
currentMonth = (new Date()).getMonth() + 1;

startYear = currentYear - 4;
endYear = currentYear + 3;

//Danh sách năm hiện tại -4 và +3

for (i = startYear; i <= endYear; i++) {
    newOption = document.createElement("option");
    newOption.value = i;
    newOption.label = i;
    if (i == currentYear) {
        newOption.selected = true;
    }
    dropdownYear.appendChild(newOption);
}

//Danh sách tháng
for (m = 1; m <= 12; m++) {
    newOption = document.createElement("option");
    newOption.value = m;
    newOption.label = m;
    if (m == currentMonth) {
        newOption.selected = true;
    }
    dropdownMonth.appendChild(newOption);
}