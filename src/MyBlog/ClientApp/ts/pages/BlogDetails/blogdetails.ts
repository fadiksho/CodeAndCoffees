window.addEventListener("load", function() {
  let scrolToButtom = document.getElementById("scrolToButton");
  if (scrolToButtom) {
    scrolToButtom.addEventListener("click", function() {
      window.scroll({
        behavior: "smooth",
        left: 0,
        top: 0
      });
    });
  }
});
