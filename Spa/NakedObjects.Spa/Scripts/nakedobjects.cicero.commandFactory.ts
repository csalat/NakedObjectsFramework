/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {

    export interface ICommandFactory {

        initialiseCommands(cvm: CiceroViewModel): void;

        parseInput(input: string, cvm: CiceroViewModel): void;

        processSingleCommand(command: string, cvm: CiceroViewModel, chained: boolean): void;

        processChain(commands: string[], cvm: CiceroViewModel): void;

        executeNextChainedCommandIfAny(cvm: CiceroViewModel): void;

        autoComplete(partialCommand: string, cvm: CiceroViewModel): void;

        //Returns all commands that may be invoked in the current context
        allCommandsForCurrentContext(): string;

        getCommand(commandWord: string): Command;
    }

    app.service('commandFactory', function (
        $q: ng.IQService,
        $location: ng.ILocationService,
        $filter: ng.IFilterService,
        $route: ng.route.IRouteService,
        $cacheFactory: ng.ICacheFactoryService,
        repLoader: IRepLoader,
        color: IColor,
        context: IContext,
        mask: IMask,
        urlManager: IUrlManager,
        focusManager: IFocusManager,
        navigation: INavigation,
        clickHandler: IClickHandler) {

        var commandFactory = <ICommandFactory>this;

        let commandsInitialised = false;

        const commands: _.Dictionary<Command> = {
            "ac": new Action(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ba": new Back(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ca": new Cancel(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "co": new Collection(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "cl": new Clipboard(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ed": new Edit(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "fi": new Field(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "fo": new Forward(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ge": new Gemini(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "go": new Go(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "he": new Help(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "me": new Menu(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ok": new OK(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "pa": new Page(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "re": new Reload(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ro": new Root(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "sa": new Save(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "se": new Selection(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "sh": new Show(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "wh": new Where(urlManager, $location, commandFactory, context, navigation, $q, $route)
        }

        commandFactory.initialiseCommands = (cvm: CiceroViewModel) => {
            if (!commandsInitialised) {
                _.forEach(commands, command => command.initialiseWithViewModel(cvm));
                commandsInitialised = true;
            }
        };

        commandFactory.parseInput = (input: string, cvm: CiceroViewModel) => {
            cvm.chainedCommands = null; //TODO: Maybe not needed if unexecuted commands are cleared down upon error?
            if (!input) { //Special case for hitting Enter with no input
                commandFactory.getCommand("wh").execute(null, false);
                return;
            }
            const commands = input.split(";");
            if (commands.length > 1) {
                commandFactory.processChain(commands, cvm);
            } else {
                commandFactory.processSingleCommand(input, cvm, false);
            }
        };

        commandFactory.processSingleCommand = (input: string, cvm: CiceroViewModel, chained: boolean) => {
            cvm.previousInput = input; //TODO: do this here?
            try {
                input = input.toLowerCase().trim();
                const firstWord = input.split(" ")[0];
                const command: Command = commandFactory.getCommand(firstWord);
                //TODO: Should previousInput be set here (given chained commands)
                cvm.previousInput = command.fullCommand + input.substring(firstWord.length, input.length);
                var argString: string = null;
                const index = input.indexOf(" ");
                if (index >= 0) {
                    argString = input.substr(index + 1);
                }
                command.execute(argString, chained);
            }
            catch (Error) {
                cvm.output = Error.message;
                cvm.input = "";
            }
        };

        commandFactory.processChain = (commands: string[], cvm: CiceroViewModel) => {
            let first = commands[0];
            commands.splice(0, 1);
            cvm.chainedCommands = commands;
            commandFactory.processSingleCommand(first, cvm, false);
        };

        commandFactory.executeNextChainedCommandIfAny = (cvm: CiceroViewModel) => {
            if (cvm.chainedCommands && cvm.chainedCommands.length > 0) {
                const next = cvm.popNextCommand();
                commandFactory.processSingleCommand(next, cvm, true);
            }
        };

        commandFactory.autoComplete = (input: string, cvm: CiceroViewModel) => {
            if (!input || input.length < 2 || input.indexOf(" ") > 0) return;
            cvm.previousInput = input;
            try {
                input = input.toLowerCase().trim();
                const command: Command = commandFactory.getCommand(input);
                cvm.input = command.fullCommand + " ";
            }
            catch (Error) {
                cvm.output = Error.message;
                cvm.input = "";
            }
        };

        commandFactory.getCommand = (commandWord: string) => {
            if (commandWord.length < 2) {
                throw new Error("Command word must have at least 2 characters");
            }
            const abbr = commandWord.substr(0, 2);
            const command: Command = commands[abbr];
            if (command == null) {
                throw new Error("No command begins with " + abbr);
            }
            command.checkMatch(commandWord);
            return command;
        }

        commandFactory.allCommandsForCurrentContext = () => {
            var result = "Commands available in current context:\n";
            for (var key in commands) {
                var c = commands[key];
                if (c.isAvailableInCurrentContext()) {
                    result = result + c.fullCommand + "\n";
                }
            }
            return result;
        }
    });
}