var gulp = require('gulp'),
    shell = require('gulp-shell');

var configuratorFiles = ['Configurator.cs'];

gulp.task('watch', function() {
    return gulp.watch(configuratorFiles, ['testconfigurator']);
});

gulp.task('testconfigurator', shell.task([
    'scriptcs ConfiguratorTests.csx'
]));
