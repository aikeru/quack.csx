var gulp = require('gulp'),
    shell = require('gulp-shell');

var configuratorFiles = ['Configurator.cs'];
var seleniumFiles = ['.\\Selenium\\TestSelenium.cs'];

gulp.task('watch', function() {
    return gulp.watch(configuratorFiles, ['testconfigurator']);
});

gulp.task('testconfigurator', shell.task([
    'scriptcs ConfiguratorTests.csx'
]));

gulp.task('watch-selenium', function() {
    return gulp.watch(seleniumFiles, ['testselenium']);
});

gulp.task('testselenium', shell.task([
    'scriptcs .\\Selenium\\TestSelenium.cs'
]));
