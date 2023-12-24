pipeline {
    agent { 
        label 'agent-jenkins'
    }
    stages {
        stage ("CleanUp Workspace"){
            steps {
                cleanWs()
            }
        }
        stage ("clone-repo-git"){
            steps {
                git branch: 'main', credentialsId: 'auth-github', url : 'https://github.com/serignemodou/devsecops-dotnet.git'
            }
        }
        stage("Sonarqube Analysis"){
            steps { 
                script {
                    def scannerHome = tool 'sonarqube-scanner-net'
                    withSonarQubeEnv(credentialsId: 'auth-sonar', installationName: 'sonarqube-server') {
                    bat "${scannerHome}\\SonarQube.Scanner.MSBuild.exe begin /k:dotnet"
                    bat "MSBuild.exe /t:Rebuild"
                    bat "${scannerHome}\\SonarQube.Scanner.MSBuild.exe end"
                    }
                }
            }
        }
    }   
}