pipeline {
    agent none
    stages {
        stage ("clone-repo-git"){
            agent "agent-jenkins"
            steps {
                echo "Connection to Github and Clone Repo"
                script {
                    withCredentials([gitUsernamePassword(credentialsId: 'auth-github', usernameVariable: 'GIT_USERNAME', passwordVariable: 'GIT_PASSWORD')]){
                        sh 'git status'
                    }
                }
            }
    }
}
}