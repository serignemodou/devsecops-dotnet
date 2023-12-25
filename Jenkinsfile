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
            environment {
                SONAR_TOKEN=credentials('auth-sonar')
                SONAR_URL="http://localhost:9000/"
                SONAR_PROJECT_KEY="dotnet"
            }
            steps { 
                script {
                    def scannerHome = tool name: 'sonarqube-scanner-net', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation'
                    withSonarQubeEnv(credentialsId: 'auth-sonar', installationName: 'sonarqube-server') {
                        sh "${tool("sonarqube-scanner-net")}/bin/sonar-scanner -X \
                        -Dsonar.host.url=http://localhost:9000 \
                        -Dsonar.login=${env.SONAR_TOKEN}"
                        env.PATH = "$PATH:/home/azureuser/.dotnet"
                        env.PATH = "$PATH:/home/azureuser/.dotnet/tools"
                        //sh "dotnet tool install --global dotnet-ef --version 7.0"
                        sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:\"dotnet\" /d:sonar.host.url=${env.SONAR_URL} /d:sonar.login=\"${env.SONAR_TOKEN}"
                        sh "dotnet build /webApi/."
                        sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"
                    }
                }
            }
        }
    }   
}