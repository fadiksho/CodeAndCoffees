import "./blog-detail.scss";

window.addEventListener("load", function() {
  document
    .getElementById("scrolToButton")
    .addEventListener("click", function() {
      window.scroll({
        behavior: "smooth",
        left: 0,
        top: pwaInstallButton.offsetTop
      });
    });
});
