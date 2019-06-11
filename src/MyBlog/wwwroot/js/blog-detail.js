window.addEventListener('load', function () {
  document.getElementById('scrolToButton').addEventListener('click', function () {
    window.scroll({
      behavior: 'smooth',
      left: 0,
      top: pwaInstallButton.offsetTop
    });
  });
});

function checkBlogDetailTheme() {
  if (userTheme) {
    if (userTheme === 'default') {
      if (!styleExistById('blog-detail-theme-default')) {
        addstyleSheetLink('/dist/css/theme/blog-detail-theme-default.css', 'blog-detail-theme-default');
        setTimeout(removeStyleSheetLink('blog-detail-theme-dark'), 2000);
      }
    } else {
      if (!styleExistById('blog-detail-theme-dark')) {
        addstyleSheetLink('/dist/css/theme/blog-detail-theme-dark.css', 'blog-detail-theme-dark');
        setTimeout(removeStyleSheetLink('blog-detail-theme-default'), 2000);
      }
    }
  }

}

themeChangeEventTarget.addEventListener('themeChangeEvent', function () {
  checkBlogDetailTheme();
});