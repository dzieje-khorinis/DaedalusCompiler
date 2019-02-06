## Deployment
To deploy new version you need make 3 things:
* First you need change version of program in main .cs version ( it's required cause build flow check if version is the same with tag )
* Add changelog in docs/changelog/changes-$VERSION.html with info what was added ( required )
* Add tag to current git head with name $VERSION and push that to github, after couple seconds deploy should be done :)