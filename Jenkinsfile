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
            echo "Scan Source Code"
            steps {
                def scannerHome = tool 'sonarqube-scanner-net'
                withSonarQubeEnv(credentialsId: 'auth-sonar') {
                    sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:dotnet"
                    sh "dotnet build"
                    sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"
                }
            }
        }
    }   
}