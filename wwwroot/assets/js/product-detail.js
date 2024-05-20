
document.addEventListener("DOMContentLoaded", function () {

    jQuery(".cake-img-slider").slick({
        vertical: true,
        verticalSwiping: true,
        dots: false,
        autoplay: true,
        arrows: false,
        infinite: true,
        speed: 300,
        slidesToShow: 5,
        slidesToScroll: 2,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 5,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: true,
                },
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 5,
                    slidesToScroll: 2,
                },
            },
            {
                breakpoint: 576,
                settings: {
                    vertical: false,
                    slidesToShow: 5,
                    slidesToScroll: 1,
                },
            },
        ],
    });

    // Get all image elements
    var images = document.querySelectorAll(".sliderImg");
    var mainImage = document.querySelector(".mainImg");

    // Add click event listener to each image
    images.forEach(function (image) {
        image.addEventListener("click", function (e) {
            e.preventDefault();
            mainImage.src = image.src;
        });
    });
});