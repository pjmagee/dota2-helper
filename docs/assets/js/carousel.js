// Select DOM elements
const track = document.querySelector('.carousel-track');
const slides = Array.from(track.children);
const nextButton = document.querySelector('.carousel-button.next');
const prevButton = document.querySelector('.carousel-button.prev');

let currentIndex = 0;

// Update carousel position
function updateCarousel() {
    const slideWidth = slides[0].getBoundingClientRect().width;
    track.style.transform = `translateX(-${currentIndex * slideWidth}px)`;
}

// Event listeners for navigation buttons
nextButton.addEventListener('click', () => {
    if (currentIndex < slides.length - 1) {
        currentIndex++;
        updateCarousel();
    }
});

prevButton.addEventListener('click', () => {
    if (currentIndex > 0) {
        currentIndex--;
        updateCarousel();
    }
});

// Initialize the carousel layout
window.addEventListener('DOMContentLoaded', () => {
    const slideWidth = slides[0].getBoundingClientRect().width;
    slides.forEach((slide, index) => {
        slide.style.left = `${index * slideWidth}px`;
    });
});

// Handle resizing to keep the layout consistent
window.addEventListener('resize', updateCarousel);
