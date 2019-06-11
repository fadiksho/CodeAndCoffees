/// <binding />
"use strict";

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  uglify = require("gulp-uglify"),
  sass = require("gulp-sass"),
  browserSync = require("browser-sync");

const server = browserSync.create();

function serve(done) {
  server.init({
    proxy: "https://localhost:44346/",
    port: 44346
  });
  done();
}
function reload(done) {
  server.reload();
  done();
}

var paths = {
  webroot: "./wwwroot/"
};

// Shared Css & Js
paths.js = paths.webroot + "js";
paths.css = paths.webroot + "css";
paths.scss = paths.webroot + "scss";
paths.distJs = paths.webroot + 'dist/js';
paths.distCss = paths.webroot + 'dist/css';

// Covert site sass to css
gulp.task('sass:site', function () {
  return gulp.src([paths.scss + '/site.scss'])
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest(paths.css))
    .pipe(browserSync.stream());
});
// Covert site sass to css
gulp.task('sass:enhancement', function () {
  return gulp.src([paths.scss + '/enhancement.scss'])
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest(paths.css))
    .pipe(browserSync.stream());
});
// Convert default theme sass to css
gulp.task('sass:theme-default', function () {
  return gulp.src([paths.scss + '/theme/theme-default.scss'])
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest(paths.css + '/theme'))
    .pipe(browserSync.stream());
});
// Convert dark theme sass to css
gulp.task('sass:theme-dark', function () {
  return gulp.src([paths.scss + '/theme/theme-dark.scss'])
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest(paths.css + '/theme'))
    .pipe(browserSync.stream());
});
// Convert default code theme Sass to css
gulp.task('sass:code-default', function () {
  return gulp.src([paths.scss + '/theme/code-default.scss'])
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest(paths.css + '/theme'))
    .pipe(browserSync.stream());
});
// Convert Blog Detail Page default theme Sass to css
gulp.task('sass:blog-detail-theme-default', function () {
  return gulp.src([paths.scss + '/theme/blog-detail-theme-default.scss'])
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest(paths.css + '/theme'))
    .pipe(browserSync.stream());
});
// Convert Blog Detail Page dark theme Sass to css
gulp.task('sass:blog-detail-theme-dark', function () {
  return gulp.src([paths.scss + '/theme/blog-detail-theme-dark.scss'])
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest(paths.css + '/theme'))
    .pipe(browserSync.stream());
});

gulp.task('sass:blog-detail', function () {
  return gulp.src([paths.scss + '/component/blog-detail.scss'])
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest(paths.css))
    .pipe(browserSync.stream());
});

// Minimize initial js
gulp.task("min:siteJs", () => {
  return gulp.src([paths.js + '/site.js'])
    .pipe(concat(paths.distJs + '/site.js'))
    .pipe(uglify())
    .pipe(gulp.dest("."));
});
// Minimize initial js
gulp.task("min:blog-detailJs", () => {
  return gulp.src([paths.js + '/blog-detail.js'])
    .pipe(concat(paths.distJs + '/blog-detail.js'))
    .pipe(uglify())
    .pipe(gulp.dest("."));
});

// Minimize initial style
gulp.task("min:siteCss", () => {
  return gulp.src([paths.css + '/site.css'])
    .pipe(concat(paths.distCss + '/site.css'))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});
// Minimize default theme
gulp.task("min:theme-default", () => {
  return gulp.src([paths.css + '/theme/theme-default.css'])
    .pipe(concat(paths.distCss + '/theme/theme-default.css'))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});
// Minimize dark theme
gulp.task("min:theme-dark", () => {
  return gulp.src([paths.css + '/theme/theme-dark.css'])
    .pipe(concat(paths.distCss + '/theme/theme-dark.css'))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});
// Minimize Blog Detail Page default theme
gulp.task("min:blog-detail-theme-default", () => {
  return gulp.src([paths.css + '/theme/blog-detail-theme-default.css'])
    .pipe(concat(paths.distCss + '/theme/blog-detail-theme-default.css'))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});
// Minimize Blog Detail Page dark theme
gulp.task("min:blog-detail-theme-dark", () => {
  return gulp.src([paths.css + '/theme/blog-detail-theme-dark.css'])
    .pipe(concat(paths.distCss + '/theme/blog-detail-theme-dark.css'))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});
// Minimize enhancement
gulp.task("min:enhancement", () => {
  return gulp.src([paths.css + '/enhancement.css'])
    .pipe(concat(paths.distCss + '/enhancement.css'))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});
// Minimize Blog Detail
gulp.task("min:blog-detail", () => {
  return gulp.src([paths.css + '/blog-detail.css'])
    .pipe(concat(paths.distCss + '/blog-detail.css'))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});
// Romve the minimized css
gulp.task("clean:js", done => rimraf(paths.distJs, done));
gulp.task("clean:css", done => rimraf(paths.distCss, done));
// Watch mode in development only
function watchChanges() {
  gulp.watch('./wwwroot/scss/**/*.scss',
    gulp.series(['sass', reload]));
  gulp.watch('./wwwroot/js/**/*.js', reload);
}
gulp.task('watch', gulp.series([serve, watchChanges]));

gulp.task("sass", gulp.series(["sass:site", "sass:enhancement", "sass:blog-detail", "sass:theme-default", "sass:theme-dark", "sass:blog-detail-theme-default", "sass:blog-detail-theme-dark"]));
gulp.task("clean", gulp.series(["clean:js", "clean:css"]));
gulp.task("min", gulp.series(["min:siteJs", "min:blog-detailJs", "min:siteCss", "min:enhancement", "min:blog-detail", "min:theme-default", "min:theme-dark", "min:blog-detail-theme-default", "min:blog-detail-theme-dark"]));
// Before deployment
gulp.task("deploy", gulp.series(["sass", "clean", "min"]));
// A 'default' task is required by Gulp v4
gulp.task("default", gulp.series(["deploy"]));
  