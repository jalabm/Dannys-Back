$(".cake-img-slider").slick({
  vertical: true,
  verticalSwiping: true,
  dots: false,
  autoplay: true,
  arrows: false,
  infinite: true,
  speed: 500,
  slidesToShow: 5,
  slidesToScroll: 1,
  responsive: [
    {
      breakpoint: 1024,
      settings: {
        slidesToShow: 5,
        slidesToScroll: 1,
        infinite: true,
        dots: true,
      },
    },
    {
      breakpoint: 600,
      settings: {
        slidesToShow: 5,
        slidesToScroll: 1,
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

const images = document.querySelectorAll(".sliderImg");
const mainImage = document.querySelector(".mainImg");

images.forEach((image) => {
  image.addEventListener("click", (e) => {
    e.preventDefault();
    mainImage.src=image.src;
  });
});



const descriptionSectionBtn=document.querySelector(".descriptionSectionBtn")
const reviewsSectionBtn=document.querySelector(".reviewsSectionBtn")


const descriptionSection=document.querySelector(".descriptionSection")
const reviewsSection=document.querySelector(".reviewsSection")


descriptionSectionBtn.addEventListener('click',(e)=>{
  e.preventDefault();

  descriptionSectionBtn.classList.add("active")
  descriptionSection.classList.add('active')

  reviewsSection.classList.remove('active')
  reviewsSectionBtn.classList.remove('active')
})



reviewsSectionBtn.addEventListener('click',(e)=>{
  e.preventDefault();

  descriptionSectionBtn.classList.remove("active")
  descriptionSection.classList.remove('active')

  reviewsSection.classList.add('active')
  reviewsSectionBtn.classList.add('active')
})