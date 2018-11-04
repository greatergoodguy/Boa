const fs = require('fs');

console.log('hello world!');

walkSync('Assets/').filter(x => x.endsWith('.cs')).forEach(lintFileSync);

/** @returns {Array<string>} */
function walkSync(dir, filelist) {
    const files = fs.readdirSync(dir);

    filelist = filelist || [];

    files.forEach((file) => {
        if (fs.statSync(dir + file).isDirectory()) {
            filelist = walkSync(dir + file + '/', filelist);
        }
        else {
            filelist.push(dir + file);
        }
    });

    return filelist;
};

function lintFileSync(fileName) {
    const fileContent = fs.readFileSync(fileName, 'utf8');
    if (fileContent.match(/Input.GetKey/) && fileName.endsWith('DG_Input.cs') === false) {
        console.error(`Use DG_Input! ${fileName}`);
    }
}
