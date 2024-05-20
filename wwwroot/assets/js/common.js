//Header

window.addEventListener("load", function () {
  var header = document.querySelector("header");
  var headerhome = this.document.querySelector(".header-home");
  var scrollPosition = window.scrollY;
  // Change background color and height when scroll position is greater than 100px
  if (scrollPosition > 40) {
    header.style.backgroundColor = "#000"; // Change background color to black
    header.style.height = "height: 80px;"; // Change header height
    headerhome.style.padding = "20px 15px ";
    // setTimeout(changeColor, 3);
  } else {
    headerhome.style.padding = " 50px 15px 20px 15px ";
    header.style.backgroundColor = "transparent"; // Change background color to transparent
  }
});

window.addEventListener("scroll", function () {
  var header = document.querySelector("header");
  var headerhome = this.document.querySelector(".header-home");
  var scrollPosition = window.scrollY;
  // Change background color and height when scroll position is greater than 100px
  if (scrollPosition > 40) {
    header.style.backgroundColor = "#000"; // Change background color to black
    header.style.height = "height: 80px;"; // Change header height
    headerhome.style.padding = "20px 15px ";
    // setTimeout(changeColor, 3);
  } else {
    headerhome.style.padding = " 50px 15px 20px 15px ";
    header.style.backgroundColor = "transparent"; // Change background color to transparent
  }
});
