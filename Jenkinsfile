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
                PROJECT_NAME="webApi"
            }
            steps { 
                script {
                    def scannerHome = tool name: 'sonarqube-scanner-net', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation'
                    withSonarQubeEnv(credentialsId: 'auth-sonar', installationName: 'sonarqube-server') {
                        env.PATH = "$PATH:/home/azureuser/.dotnet"
                        env.PATH = "$PATH:/home/azureuser/.dotnet/tools"
                        //sh "dotnet tool install --global dotnet-ef --version 7.0"
                        sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:\"devsecops-dotnet\""
                        sh "dotnet build $PROJECT_NAME"
                        sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"
                    }
                }
            }
        }
        /*
        stage("Quality Gate") {
            steps{
                waitForQualityGate abortPipeline: false , credentialsId: 'auth-sonar'
                timeout(time: 5, unit: 'MINUTES') {
                    script{
                        def quality_gate = waitForQualityGate()
                        if (quality_gate.status != 'OK'){
                            error "Pipeline aborted due to quality gate failure: ${quality_gate.status}"
                        }
                    }
                }
            }
        }*/
        stage("Owasp Dependency Check Vulnerabilities"){
            steps{
                dependencyCheck additionalArguments '''
                    -o './'
                    -s './webApi'
                    -f 'ALL'
                    --prettyPrint''', odcInstallation: 'Owasp-dependency-check'
                dependencyCheckPublisher pattern: 'dependency-check-report.xml'
            }
        }
    }   
}